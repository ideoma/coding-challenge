using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ConstructionLine.CodingChallenge.Tests
{
    [TestFixture]
    public class SearchEngineTests : SearchEngineTestsBase
    {
        [Test]
        public void Test()
        {
            var shirts = new List<Shirt>
            {
                new Shirt(Guid.NewGuid(), "Red - Small", Size.Small, Color.Red),
                new Shirt(Guid.NewGuid(), "Black - Medium", Size.Medium, Color.Black),
                new Shirt(Guid.NewGuid(), "Blue - Large", Size.Large, Color.Blue),
            };

            var searchEngine = new SearchEngine(shirts);

            var searchOptions = new SearchOptions
            {
                Colors = new List<Color> {Color.Red},
                Sizes = new List<Size> {Size.Small}
            };

            var results = searchEngine.Search(searchOptions);

            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(results.Shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(results.Shirts, searchOptions, results.ColorCounts);
        }
        
        [Test]
        public void ShouldThrowArgumentExceptionWhenInitializedWithNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SearchEngine(null));
        }
        
        [Test]
        public void ShouldThrowArgumentExceptionWhenSearchedWithNull()
        {
            var engine = new SearchEngine(new List<Shirt>());
            Assert.Throws<ArgumentNullException>(() => engine.Search(null));
        }
        
        [Test]
        public void ShouldThrowArgumentExceptionWhenSearchedWithNullColors()
        {
            var engine = new SearchEngine(new List<Shirt>());
            Assert.Throws<ArgumentNullException>(() => engine.Search(
                    new SearchOptions
                    {
                        Colors = null
                    }
                ));
        }

        [Test]
        public void ShouldThrowArgumentExceptionWhenSearchedWithNullSizes()
        {
            var engine = new SearchEngine(new List<Shirt>());
            Assert.Throws<ArgumentNullException>(() => engine.Search(
                new SearchOptions
                {
                    Sizes = null
                }
            ));
        }

        private const int Sizes = 3;
        private const int Colors = 5;
        
        [TestCase(2, "Red", "Medium", 2)]
        [TestCase(2, "Red", "Medium,Large", 2*1*2)]
        [TestCase(2, "Red,Yellow", "Medium,Large", 2*2*2)]
        [TestCase(2, "Red,White", "", 2*2*Sizes)]
        [TestCase(1, "", "Medium,Small,Large", 1*Colors*3)]
        [TestCase(1, "", "Large", 1*Colors*1)]
        [TestCase(1, "", "", 1*Colors*Sizes)]
        [TestCase(10, "", "", 10*Colors*Sizes)]
        [TestCase(1000, "Black,Blue,Yellow", "", 1000*3*Sizes)]
        [TestCase(0, "Black,Blue,Yellow", "", 0*3*Sizes)]
        public void ShouldCalculateShirtsCorrectlyUniformFill(int shirtsInEachCategory, string colors, string sizes, int expectedShirts)
        {
            var shirts = GenerateShirtsUniform(shirtsInEachCategory);
            var engine = new SearchEngine(shirts);
            var searchOptions = new SearchOptions
            {
                Colors = ToColorList(colors),
                Sizes = ToSizesList(sizes)
            };
            
            var results = engine.Search(searchOptions);
            
            Assert.That(results.Shirts.Count, Is.EqualTo(expectedShirts));
            AssertResults(results.Shirts, searchOptions);
            AssertSizeCounts(results.Shirts, searchOptions, results.SizeCounts);
            AssertColorCounts(results.Shirts, searchOptions, results.ColorCounts);
        }

        private List<Shirt> GenerateShirtsUniform(int count)
        {
            return Color.All.SelectMany(
                c => Size.All.SelectMany(s =>
                    Enumerable.Range(0, count).Select(id => 
                        new Shirt(Guid.NewGuid(), $"{c.Name} - {s.Name} - {id}", s, c))
                )
            ).ToList();
        }
    }
}
