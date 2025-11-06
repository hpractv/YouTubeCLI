using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using YouTubeCLI.Commands;
using YouTubeCLI.Libraries;

namespace YouTubeCLI.Tests.Commands
{
    public class EnumAndConstantsTests
    {
        [Fact]
        public void PrivacyEnum_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            var privacyValues = Enum.GetValues<PrivacyEnum>();

            // Assert
            privacyValues.Should().HaveCount(3);
            privacyValues.Should().Contain(PrivacyEnum.Private);
            privacyValues.Should().Contain(PrivacyEnum.Public);
            privacyValues.Should().Contain(PrivacyEnum.Unlisted);
        }

        [Theory]
        [InlineData(PrivacyEnum.Private, "Private")]
        [InlineData(PrivacyEnum.Public, "Public")]
        [InlineData(PrivacyEnum.Unlisted, "Unlisted")]
        public void PrivacyEnum_ShouldHaveCorrectStringRepresentation(PrivacyEnum privacy, string expectedString)
        {
            // Act
            var actualString = privacy.ToString();

            // Assert
            actualString.Should().Be(expectedString);
        }

        [Fact]
        public void OutputOptionsEnum_ShouldHaveCorrectValues()
        {
            // Arrange & Act
            var outputValues = Enum.GetValues<OutputOptionsEnum>();

            // Assert
            outputValues.Should().HaveCount(5);
            outputValues.Should().Contain(OutputOptionsEnum.Single);
            outputValues.Should().Contain(OutputOptionsEnum.Monthly);
            outputValues.Should().Contain(OutputOptionsEnum.Daily);
            outputValues.Should().Contain(OutputOptionsEnum.Hourly);
            outputValues.Should().Contain(OutputOptionsEnum.Broadcast);
        }

        [Theory]
        [InlineData(OutputOptionsEnum.Single, "Single")]
        [InlineData(OutputOptionsEnum.Monthly, "Monthly")]
        [InlineData(OutputOptionsEnum.Daily, "Daily")]
        [InlineData(OutputOptionsEnum.Hourly, "Hourly")]
        [InlineData(OutputOptionsEnum.Broadcast, "Broadcast")]
        public void OutputOptionsEnum_ShouldHaveCorrectStringRepresentation(OutputOptionsEnum output, string expectedString)
        {
            // Act
            var actualString = output.ToString();

            // Assert
            actualString.Should().Be(expectedString);
        }

        [Fact]
        public void Constants_ShouldHaveCorrectColumnNames()
        {
            // Assert
            Constants.YouTubeId_COLUMN.Should().Be("YouTubeId");
            Constants.Title_COLUMN.Should().Be("Title");
            Constants.Start_COLUMN.Should().Be("Start");
            Constants.AutoStart_COLUMN.Should().Be("AutoStart");
            Constants.AutoStop_COLUMN.Should().Be("AutoStop");
            Constants.Privacy_COLUMN.Should().Be("Privacy");
            Constants.ChatEnabled_COLUMN.Should().Be("ChatEnabled");
            Constants.Url_COLUMN.Should().Be("URL");
            Constants.Link_COLUMN.Should().Be("Link");
        }

        [Fact]
        public void Constants_COLUMNS_ShouldContainAllColumnNames()
        {
            // Arrange
            var expectedColumns = new[]
            {
                Constants.YouTubeId_COLUMN,
                Constants.Title_COLUMN,
                Constants.Start_COLUMN,
                Constants.AutoStart_COLUMN,
                Constants.AutoStop_COLUMN,
                Constants.Privacy_COLUMN,
                Constants.ChatEnabled_COLUMN,
                Constants.Url_COLUMN,
                Constants.Link_COLUMN
            };

            // Act & Assert
            Constants.COLUMNS.Should().HaveCount(9);
            Constants.COLUMNS.Should().BeEquivalentTo(expectedColumns);
        }

        [Fact]
        public void Constants_COLUMNS_ShouldBeInCorrectOrder()
        {
            // Arrange
            var expectedOrder = new[]
            {
                "YouTubeId",
                "Title",
                "Start",
                "AutoStart",
                "AutoStop",
                "Privacy",
                "ChatEnabled",
                "URL",
                "Link"
            };

            // Act & Assert
            Constants.COLUMNS.Should().Equal(expectedOrder);
        }

        [Fact]
        public void PrivacyEnum_ShouldBeComparable()
        {
            // Arrange
            var private1 = PrivacyEnum.Private;
            var private2 = PrivacyEnum.Private;
            var public1 = PrivacyEnum.Public;

            // Act & Assert
            private1.Should().Be(private2);
            private1.Should().NotBe(public1);
            (private1 == private2).Should().BeTrue();
            (private1 != public1).Should().BeTrue();
        }

        [Fact]
        public void OutputOptionsEnum_ShouldBeComparable()
        {
            // Arrange
            var single1 = OutputOptionsEnum.Single;
            var single2 = OutputOptionsEnum.Single;
            var monthly = OutputOptionsEnum.Monthly;

            // Act & Assert
            single1.Should().Be(single2);
            single1.Should().NotBe(monthly);
            (single1 == single2).Should().BeTrue();
            (single1 != monthly).Should().BeTrue();
        }

        [Fact]
        public void PrivacyEnum_ShouldSupportEnumParsing()
        {
            // Act & Assert
            Enum.Parse<PrivacyEnum>("Private").Should().Be(PrivacyEnum.Private);
            Enum.Parse<PrivacyEnum>("Public").Should().Be(PrivacyEnum.Public);
            Enum.Parse<PrivacyEnum>("Unlisted").Should().Be(PrivacyEnum.Unlisted);
        }

        [Fact]
        public void OutputOptionsEnum_ShouldSupportEnumParsing()
        {
            // Act & Assert
            Enum.Parse<OutputOptionsEnum>("Single").Should().Be(OutputOptionsEnum.Single);
            Enum.Parse<OutputOptionsEnum>("Monthly").Should().Be(OutputOptionsEnum.Monthly);
            Enum.Parse<OutputOptionsEnum>("Daily").Should().Be(OutputOptionsEnum.Daily);
            Enum.Parse<OutputOptionsEnum>("Hourly").Should().Be(OutputOptionsEnum.Hourly);
            Enum.Parse<OutputOptionsEnum>("Broadcast").Should().Be(OutputOptionsEnum.Broadcast);
        }

        [Fact]
        public void PrivacyEnum_ShouldSupportCaseInsensitiveParsing()
        {
            // Act & Assert
            Enum.Parse<PrivacyEnum>("private", true).Should().Be(PrivacyEnum.Private);
            Enum.Parse<PrivacyEnum>("PUBLIC", true).Should().Be(PrivacyEnum.Public);
            Enum.Parse<PrivacyEnum>("UnListed", true).Should().Be(PrivacyEnum.Unlisted);
        }

        [Fact]
        public void OutputOptionsEnum_ShouldSupportCaseInsensitiveParsing()
        {
            // Act & Assert
            Enum.Parse<OutputOptionsEnum>("single", true).Should().Be(OutputOptionsEnum.Single);
            Enum.Parse<OutputOptionsEnum>("MONTHLY", true).Should().Be(OutputOptionsEnum.Monthly);
            Enum.Parse<OutputOptionsEnum>("Daily", true).Should().Be(OutputOptionsEnum.Daily);
            Enum.Parse<OutputOptionsEnum>("hourly", true).Should().Be(OutputOptionsEnum.Hourly);
            Enum.Parse<OutputOptionsEnum>("BROADCAST", true).Should().Be(OutputOptionsEnum.Broadcast);
        }
    }
}
