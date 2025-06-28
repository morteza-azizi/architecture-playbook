using NetArchTest.Rules;
using Xunit;

namespace NetArchTestSample.ArchitectureTests;

public class ArchitecturalRules
{
    private const string DomainNamespace = "NetArchTestSample.Domain";
    private const string InfrastructureNamespace = "NetArchTestSample.Infrastructure";
    private const string DataNamespace = "NetArchTestSample.Data";
    private const string ApiNamespace = "NetArchTestSample.Api";

    [Fact]
    public void Domain_Should_Not_Depend_On_Infrastructure()
    {
        var validationResult = Types.InAssembly(typeof(Domain.Entities.Book).Assembly)
            .That().ResideInNamespace(DomainNamespace)
            .ShouldNot().HaveDependencyOn(InfrastructureNamespace)
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Domain layer should not depend on Infrastructure. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Data()
    {
        var validationResult = Types.InAssembly(typeof(Domain.Entities.Book).Assembly)
            .That().ResideInNamespace(DomainNamespace)
            .ShouldNot().HaveDependencyOn(DataNamespace)
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Domain layer should not depend on Data. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_Should_Not_Depend_On_Api()
    {
        var validationResult = Types.InAssembly(typeof(Domain.Entities.Book).Assembly)
            .That().ResideInNamespace(DomainNamespace)
            .ShouldNot().HaveDependencyOn(ApiNamespace)
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Domain layer should not depend on API. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Infrastructure_Should_Not_Depend_On_Api()
    {
        var validationResult = Types.InAssembly(typeof(Infrastructure.Services.BookService).Assembly)
            .That().ResideInNamespace(InfrastructureNamespace)
            .ShouldNot().HaveDependencyOn(ApiNamespace)
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Infrastructure layer should not depend on API. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Data_Should_Not_Depend_On_Infrastructure()
    {
        var validationResult = Types.InAssembly(typeof(Data.Repositories.InMemoryBookRepository).Assembly)
            .That().ResideInNamespace(DataNamespace)
            .ShouldNot().HaveDependencyOn(InfrastructureNamespace)
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Data layer should not depend on Infrastructure. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Data_Should_Not_Depend_On_Api()
    {
        var validationResult = Types.InAssembly(typeof(Data.Repositories.InMemoryBookRepository).Assembly)
            .That().ResideInNamespace(DataNamespace)
            .ShouldNot().HaveDependencyOn(ApiNamespace)
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Data layer should not depend on API. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Controllers_Should_Not_Depend_On_Infrastructure()
    {
        var validationResult = Types.InAssembly(typeof(Api.Controllers.BooksController).Assembly)
            .That().ResideInNamespace($"{ApiNamespace}.Controllers")
            .And().HaveNameEndingWith("Controller")
            .ShouldNot().HaveDependencyOn(InfrastructureNamespace)
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Controllers should not depend on Infrastructure. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Controllers_Should_Not_Depend_On_Data()
    {
        var validationResult = Types.InAssembly(typeof(Api.Controllers.BooksController).Assembly)
            .That().ResideInNamespace($"{ApiNamespace}.Controllers")
            .And().HaveNameEndingWith("Controller")
            .ShouldNot().HaveDependencyOn(DataNamespace)
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Controllers should not depend on Data. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Repository_Classes_Should_Be_In_Data_Namespace()
    {
        var validationResult = Types.InAssembly(typeof(Data.Repositories.InMemoryBookRepository).Assembly)
            .That().HaveNameEndingWith("Repository")
            .Should().ResideInNamespace($"{DataNamespace}.Repositories")
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Repository classes should be in Data.Repositories namespace. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Service_Classes_Should_Be_In_Infrastructure_Namespace()
    {
        var validationResult = Types.InAssembly(typeof(Infrastructure.Services.BookService).Assembly)
            .That().HaveNameEndingWith("Service")
            .And().AreNotInterfaces()
            .Should().ResideInNamespace($"{InfrastructureNamespace}.Services")
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Service classes should be in Infrastructure.Services namespace. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void All_Service_Classes_Should_Be_Sealed()
    {
        var validationResult = Types.InAssembly(typeof(Infrastructure.Services.BookService).Assembly)
            .That().HaveNameEndingWith("Service")
            .And().AreNotInterfaces()
            .Should().BeSealed()
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Service classes should be sealed. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void All_Repository_Classes_Should_Be_Sealed()
    {
        var validationResult = Types.InAssembly(typeof(Data.Repositories.InMemoryBookRepository).Assembly)
            .That().HaveNameEndingWith("Repository")
            .And().AreNotInterfaces()
            .Should().BeSealed()
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Repository classes should be sealed. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void All_Controllers_Should_Be_Sealed()
    {
        var validationResult = Types.InAssembly(typeof(Api.Controllers.BooksController).Assembly)
            .That().HaveNameEndingWith("Controller")
            .Should().BeSealed()
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Controller classes should be sealed. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }


    [Fact]
    public void Interfaces_Should_Start_With_I()
    {
        var validationResult = Types.InCurrentDomain()
            .That().AreInterfaces()
            .Should().HaveNameStartingWith("I")
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Interfaces should start with 'I'. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }

    [Fact]
    public void Domain_Services_Should_Only_Use_Async_Methods()
    {
        var validationResult = Types.InAssembly(typeof(Domain.Interfaces.IBookService).Assembly)
            .That().ResideInNamespace($"{DomainNamespace}.Interfaces")
            .And().HaveNameEndingWith("Service")
            .Should().MeetCustomRule(new AsyncMethodRule())
            .GetResult();
        
        Assert.True(validationResult.IsSuccessful, 
            $"Domain service interfaces should only expose async methods. Violations: {string.Join(", ", validationResult.FailingTypeNames ?? [])}");
    }
} 
