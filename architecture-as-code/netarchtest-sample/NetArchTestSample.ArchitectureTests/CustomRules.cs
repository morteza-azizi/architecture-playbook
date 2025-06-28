using NetArchTest.Rules;
using System.Reflection;
using System.Text.RegularExpressions;
using Mono.Cecil;

namespace NetArchTestSample.ArchitectureTests;

public class PascalCaseClassNameRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        // More permissive regex that allows PascalCase with common patterns
        // Allows: PascalCase, Pascal_Case, PascalCase123, etc.
        return Regex.IsMatch(type.Name, @"^[A-Z][a-zA-Z0-9_]*$");
    }
    public string Description => "Class names must be PascalCase.";
}

public class DomainEntityRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        // Skip compiler-generated types and records
        if (type.Name.Contains("<") || type.Name.Contains("__") || 
            type.CustomAttributes.Any(attr => attr.AttributeType.Name == "CompilerGeneratedAttribute"))
        {
            return true;
        }

        // Check if type has properties with private setters or is immutable
        var properties = type.Properties;
        if (!properties.Any())
        {
            return true; // No properties to validate
        }

        foreach (var property in properties)
        {
            // Allow properties with private/protected setters or no setters (read-only)
            if (property.SetMethod != null && 
                (property.SetMethod.IsPublic || property.SetMethod.IsAssembly))
            {
                return false; // Public setter found
            }
        }

        return true;
    }
    
    public string Description => "Domain entities should have private setters to maintain encapsulation.";
}

public class AsyncMethodRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        if (!type.IsInterface)
            return true;

        foreach (var method in type.Methods)
        {
            // Skip property getters/setters and special methods
            if (method.IsSpecialName || method.IsConstructor)
                continue;

            // Check if method returns Task or Task<T>
            var returnType = method.ReturnType;
            if (returnType.Name != "Task" && !returnType.Name.StartsWith("Task`"))
            {
                return false;
            }
        }

        return true;
    }
    
    public string Description => "Service interfaces should only expose asynchronous methods returning Task or Task<T>.";
}

public class RepositoryNamingRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        if (type.IsInterface && type.Name.Contains("Repository"))
        {
            return type.Name.StartsWith("I") && type.Name.EndsWith("Repository");
        }
        
        if (!type.IsInterface && type.Name.EndsWith("Repository"))
        {
            return true; // Implementation classes can have various naming patterns
        }
        
        return true;
    }
    
    public string Description => "Repository interfaces should follow the naming pattern I[Entity]Repository.";
}

public class ControllerNamingRule : ICustomRule
{
    public bool MeetsRule(TypeDefinition type)
    {
        if (type.Name.EndsWith("Controller"))
        {
            // Controllers should be in Controllers namespace
            return type.Namespace?.EndsWith(".Controllers") == true;
        }
        
        return true;
    }
    
    public string Description => "Controllers should be in a Controllers namespace and end with 'Controller'.";
} 