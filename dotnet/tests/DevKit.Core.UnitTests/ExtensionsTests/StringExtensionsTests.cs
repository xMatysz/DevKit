using DevKit.Core.Extensions;
using FluentAssertions;
using Xunit;

namespace DevKit.Core.UnitTests.ExtensionsTests;

public sealed class StringExtensionsTests
{
    private const string ValidString = "ValidString";

    public static readonly IEnumerable<TheoryDataRow<string?>> InvalidStringsData =
    [
        new(string.Empty) { TestDisplayName = "empty string" },
        new(null) { TestDisplayName = "default | null" },
    ];

    [Theory]
    [MemberData(nameof(InvalidStringsData))]
    public void IsNullOrWhiteSpace_ShouldBe_True_For_InvalidString(string? value)
    {
        value.IsNullOrWhiteSpace().Should().BeTrue();
    }

    [Fact]
    public void IsNullOrWhiteSpace_ShouldBe_False_For_ValidString()
    {
        ValidString.IsNullOrWhiteSpace().Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(InvalidStringsData))]
    public void IsNotNullOrWhiteSpace_ShouldBe_For_For_InvalidString(string? value)
    {
        value.IsNotNullOrWhiteSpace().Should().BeFalse();
    }

    [Fact]
    public void IsNotNullOrWhiteSpace_ShouldBe_True_For_ValidString()
    {
        ValidString.IsNotNullOrWhiteSpace().Should().BeTrue();
    }
}
