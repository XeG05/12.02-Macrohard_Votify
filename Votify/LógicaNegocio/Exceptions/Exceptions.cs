namespace Votify.LógicaNegocio.Exceptions;

public class NotFoundException : Exception { public NotFoundException(string m) : base(m) { } }
public class UnauthorizedException : Exception { public UnauthorizedException(string m) : base(m) { } }
public class ConflictException : Exception { public ConflictException(string m) : base(m) { } }
public class ValidationException : Exception { public ValidationException(string m) : base(m) { } }