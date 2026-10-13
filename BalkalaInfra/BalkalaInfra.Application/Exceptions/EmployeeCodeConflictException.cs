namespace BalkalaInfra.Application.Exceptions;

public sealed class EmployeeCodeConflictException(string employeeCode)
    : Exception($"Employee code '{employeeCode}' is already in use.");
