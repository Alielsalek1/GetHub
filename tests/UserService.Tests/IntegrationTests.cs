using System;
using System.Threading.Tasks;
using FluentAssertions;
using userService.DTOs;
using userService.interfaces;
using userService.services;
using Xunit;
using userService.repositories;
using Shared.Enums;

namespace GetHub.Tests.UserService;

public class IntegrationTests
{
    private static IUserService BuildService(IUserRepository repo) => new userService.services.UserService(repo);

    [Fact]
    public async Task CreateUser_Succeeds_WhenNew()
    {
        var repo = new InMemoryUserRepository();
        var service = BuildService(repo);
        var userId = Guid.NewGuid();

        var result = await service.CreateUserAsync(new CreateUserRequest
        {
            phoneNumber = "+10000000000",
            address = "Earth"
        }, userId);

        result.IsSuccess.Should().BeTrue();

        // verify persisted state via GetUserById
        var fetched = await service.GetUserByIdAsync(userId);
        fetched.IsSuccess.Should().BeTrue();
        fetched.Value.id.Should().Be(userId);
        fetched.Value.phoneNumber.Should().Be("+10000000000");
        fetched.Value.address.Should().Be("Earth");
    }

    [Fact]
    public async Task CreateUser_Fails_WhenDuplicate()
    {
        var repo = new InMemoryUserRepository();
        var service = BuildService(repo);
        var userId = Guid.NewGuid();

        var first = await service.CreateUserAsync(new CreateUserRequest
        {
            phoneNumber = "+10000000001",
            address = "Mars"
        }, userId);
        first.IsSuccess.Should().BeTrue();

        var second = await service.CreateUserAsync(new CreateUserRequest
        {
            phoneNumber = "+10000000001",
            address = "Mars"
        }, userId);

        second.IsSuccess.Should().BeFalse();
        second.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.USER_ALREADY_EXISTS);
    }

    [Fact]
    public async Task GetUserById_Returns_User_WhenExists()
    {
        var repo = new InMemoryUserRepository();
        var service = BuildService(repo);
        var userId = Guid.NewGuid();

        await service.CreateUserAsync(new CreateUserRequest
        {
            phoneNumber = "+10000000002",
            address = "Venus"
        }, userId);

        var res = await service.GetUserByIdAsync(userId);
        res.IsSuccess.Should().BeTrue();
        res.Value.id.Should().Be(userId);
    }

    [Fact]
    public async Task UpdateUser_Updates_Only_Provided_Fields()
    {
        var repo = new InMemoryUserRepository();
        var service = BuildService(repo);
        var userId = Guid.NewGuid();

        await service.CreateUserAsync(new CreateUserRequest
        {
            phoneNumber = "+10000000003",
            address = "Mercury"
        }, userId);

        var update = await service.UpdateUserAsync(userId, new UpdateUserRequest
        {
            bio = "Hello",
            profileImageUrl = "http://img",
            bankAccountNumber = "123",
        });

        update.IsSuccess.Should().BeTrue();

        // verify persisted state via GetUserById
        var fetched = await service.GetUserByIdAsync(userId);
        fetched.IsSuccess.Should().BeTrue();
        fetched.Value.bio.Should().Be("Hello");
        fetched.Value.profileImageUrl.Should().Be("http://img");
        fetched.Value.bankAccountNumber.Should().Be("123");
        // phone and address unchanged
        fetched.Value.phoneNumber.Should().Be("+10000000003");
        fetched.Value.address.Should().Be("Mercury");
    }

    [Fact]
    public async Task DeleteUser_Removes_User()
    {
        var repo = new InMemoryUserRepository();
        var service = BuildService(repo);
        var userId = Guid.NewGuid();

        await service.CreateUserAsync(new CreateUserRequest
        {
            phoneNumber = "+10000000004",
            address = "Jupiter"
        }, userId);

        var del = await service.DeleteUserAsync(userId);
        del.IsSuccess.Should().BeTrue();

        var get = await service.GetUserByIdAsync(userId);
        get.IsSuccess.Should().BeFalse();
        get.Errors.Should().ContainSingle()
            .Which.Metadata.Should().ContainKey("errorCode")
            .WhoseValue.Should().Be(ErrorCodes.USER_NOT_FOUND);
    }
}
