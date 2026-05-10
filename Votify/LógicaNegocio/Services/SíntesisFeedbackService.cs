using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Votify.LógicaNegocio.Services
{
    public class SíntesisFeedbackService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;

        public SíntesisFeedbackService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            // Usando la versión Gemini 2.5 Flash oficial listada en tu tenant
            _apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
        }

        public async Task<string> SintetizarCriterio(string[] comments)
        {
            if (comments == null || comments.Length == 0)
                return string.Empty;

            var cleanComments = LimpiarTextos(comments);
            var mergedComments = string.Join(" | ", cleanComments);

            // Evitar pasarnos si son demasiados comentarios (Fallback crudo: 4 chars aprox 1 token)
            mergedComments = TruncateByTokensFallback(mergedComments, 2000); 

            var prompt = "Sintetiza de manera objetiva, detallada y completa la siguiente lista de comentarios para un solo criterio. No incluyas introducciones ni conclusiones. Genera un párrafo o puntos clave con suficiente nivel de detalle.";

            // Aumentado maxTokens drásticamente 5 veces la cantidad anterior
            return await RealizarPeticionGemini(prompt, mergedComments, maxTokens: 2000);
        }

        public async Task<string> SintetizarCategoria(string[] votanteComments)
        {
            if (votanteComments == null || votanteComments.Length == 0)
                return string.Empty;

            var cleanSummaries = LimpiarTextos(votanteComments);
            var mergedSummaries = string.Join(" | ", cleanSummaries);

            mergedSummaries = TruncateByTokensFallback(mergedSummaries, 3000);

            var prompt = "Genera una síntesis global, detallada y completa para este proyecto destacando los puntos clave generales mencionados en los comentarios. Retorna solo el texto abstracto sin introducciones ni despedidas, asegurándote de cubrir todos los aspectos importantes.";

            // Aumentado maxTokens drásticamente 5 veces la cantidad anterior
            return await RealizarPeticionGemini(prompt, mergedSummaries, maxTokens: 4000);
        }

        public async Task<string> ObtenerSintesisCriterioAsync(Votify.Persistencia.Entities.Proyecto proyecto, long idCriterio, string[] comments)
        {
            var sintesisExistente = proyecto.SintesisCriteriosJurado?.FirstOrDefault(s => s.IdCriterio == idCriterio);
            if (sintesisExistente != null && !string.IsNullOrEmpty(sintesisExistente.Sintesis))
            {
                Console.WriteLine($"[SintesisFeedbackService] Sintesis EXISTENTE encontrada para criterio {idCriterio}. Devolviendo cache.");
                return sintesisExistente.Sintesis;
            }

            Console.WriteLine($"[SintesisFeedbackService] Sintesis NO existe para criterio {idCriterio}. Generando nueva...");
            var nuevaSintesis = await SintetizarCriterio(comments);

            if (!string.IsNullOrEmpty(nuevaSintesis))
            {
                // Asegurar que la colección esté inicializada
                if (proyecto.SintesisCriteriosJurado == null)
                    proyecto.SintesisCriteriosJurado = new List<Votify.Persistencia.Entities.SintesisJuradoCriterio>();

                if (sintesisExistente != null)
                {
                    sintesisExistente.Sintesis = nuevaSintesis;
                }
                else
                {
                    proyecto.SintesisCriteriosJurado.Add(new Votify.Persistencia.Entities.SintesisJuradoCriterio 
                    { 
                        IdCriterio = idCriterio, 
                        IdProyecto = proyecto.Id,
                        Sintesis = nuevaSintesis 
                    });
                }
            }

            return nuevaSintesis;
        }

        public async Task<string> ObtenerSintesisVotantesAsync(Votify.Persistencia.Entities.Proyecto proyecto, string[] comments)
        {
            if (!string.IsNullOrEmpty(proyecto.SintesisVotantes))
            {
                return proyecto.SintesisVotantes;
            }

            var nuevaSintesis = await SintetizarCategoria(comments);

            if (!string.IsNullOrEmpty(nuevaSintesis))
            {
                proyecto.SintesisVotantes = nuevaSintesis;
            }

            return nuevaSintesis;
        }

        private string[] LimpiarTextos(string[] textos)
        {
            return textos.Select(texto => 
            {
                if (string.IsNullOrWhiteSpace(texto)) return "";

                // Eliminar tags HTML
                var sinHtml = Regex.Replace(texto, "<.*?>", string.Empty);
                // Quitar saltos de línea y tabuladores
                var sinSaltos = Regex.Replace(sinHtml, @"\r\n?|\n|\t", " ");
                // Reducir espacios múltiples a uno
                var limpio = Regex.Replace(sinSaltos, @"\s+", " ").Trim();

                return limpio;
            }).ToArray();
        }

        private string TruncateByTokensFallback(string text, int maxTokensLimit)
        {
            // Ratio aproximado 1 token = 4 caracteres
            int maxLength = maxTokensLimit * 4;
            if (text.Length > maxLength)
            {
                return text.Substring(0, maxLength);
            }
            return text;
        }

        private async Task<string> RealizarPeticionGemini(string systemPrompt, string userContent, int maxTokens)
        {
            Console.WriteLine($"[SintesisFeedbackService] Contenido enviado: {userContent}");

            var requestBody = new
            {
                contents = new[]
                {
                    new 
                    { 
                        role = "user", 
                        parts = new[] 
                        { 
                            new { text = $"{systemPrompt}\n\nTexto a analizar:\n{userContent}" } 
                        } 
                    }
                },
                generationConfig = new
                {
                    temperature = 0.3,
                    maxOutputTokens = maxTokens
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl);
            request.Content = JsonContent.Create(requestBody);

            Console.WriteLine($"[SintesisFeedbackService] Realizando peticion a Gemini para url: {_apiUrl.Substring(0, Math.Min(_apiUrl.Length, 50))}...");
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Configurar JsonSerializer para ignorar mayúsculas/minúsculas en JSON property names (Gemini a veces devuelve en camelCase sin guion bajo, etc)
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<GeminiResponse>(responseContent, jsonOptions);

                var message = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
                Console.WriteLine($"[SintesisFeedbackService] Peticion OK. Sintesis generada de {message?.Length ?? 0} caracteres.");
                return message?.Trim() ?? string.Empty;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[SintesisFeedbackService] Petición fallida. StatusCode: {response.StatusCode}. Detalle: {errorContent}");
            }

            // Manejo de error o log
            return string.Empty;
        }

        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public List<GeminiCandidate> Candidates { get; set; }
        }

        private class GeminiCandidate
        {
            [JsonPropertyName("content")]
            public GeminiContent Content { get; set; }
        }

        private class GeminiContent
        {
            [JsonPropertyName("parts")]
            public List<GeminiPart> Parts { get; set; }
        }

        private class GeminiPart
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
        }
    }
}
