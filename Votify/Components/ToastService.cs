namespace Votify.Components;

public enum ToastType
{
    Success,
    Error,
    Info,
    Loading
}

public class ToastMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Text { get; init; } = "";
    public ToastType Type { get; init; }
    public int? AutoDismissMs { get; init; }
}

public class ToastService
{
    public event Action? OnChange;

    public List<ToastMessage> Messages { get; } = new();

    public Guid ShowSuccess(string text, int autoDismissMs = 3500)
        => Push(new ToastMessage { Text = text, Type = ToastType.Success, AutoDismissMs = autoDismissMs });

    public Guid ShowError(string text, int autoDismissMs = 6000)
        => Push(new ToastMessage { Text = text, Type = ToastType.Error, AutoDismissMs = autoDismissMs });

    public Guid ShowInfo(string text, int autoDismissMs = 4000)
        => Push(new ToastMessage { Text = text, Type = ToastType.Info, AutoDismissMs = autoDismissMs });

    public Guid ShowLoading(string text)
        => Push(new ToastMessage { Text = text, Type = ToastType.Loading, AutoDismissMs = null });

    public void Dismiss(Guid id)
    {
        var found = Messages.FirstOrDefault(m => m.Id == id);
        if (found != null)
        {
            Messages.Remove(found);
            OnChange?.Invoke();
        }
    }

    private Guid Push(ToastMessage message)
    {
        Messages.Add(message);
        OnChange?.Invoke();

        if (message.AutoDismissMs.HasValue)
        {
            var ms = message.AutoDismissMs.Value;
            _ = Task.Run(async () =>
            {
                await Task.Delay(ms);
                Dismiss(message.Id);
            });
        }

        return message.Id;
    }
}
