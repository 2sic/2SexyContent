﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web.Images;

namespace ToSic.Sxc.Tests.ImageSrcSetTests
{
    [TestClass]
    public class SrcSetParsePart
    {
        // Basic Number and optional Width only
        [DataRow("100", 100, 'w', 100, 0)]
        [DataRow("100w", 100, 'w', 100, 0)]
        [DataRow("2000.4", 2000, 'w', 2000, 0)]
        [DataRow("2000w", 2000, 'w', 2000, 0)]
        [DataRow("4w", 4, 'w', 4, 0)]   // Very small number, without the 'w' it would default to x

        // Basic W number with additional width
        [DataRow("100=100", 100, 'w', 100, 0)]
        [DataRow("100=700", 100, 'w', 700, 0)]
        [DataRow("100w=700", 100, 'w', 700, 0)]
        [DataRow("100=700.9", 100, 'w', 701, 0)]

        // Basic W number with additional width and height
        [DataRow("100=100:50", 100, 'w', 100, 50)]
        [DataRow("100=700:300", 100, 'w', 700, 300)]
        [DataRow("100w=700:400", 100, 'w', 700, 400)]
        [DataRow("100=700.9:201.7", 100, 'w', 701, 202)]

        // Basic W number with no width but height
        [DataRow("100=:50", 100, 'w', 100, 50)]
        [DataRow("100=:300", 100, 'w', 100, 300)]
        [DataRow("100w=:400", 100, 'w', 100, 400)]
        [DataRow("100=:201.7", 100, 'w', 100, 202)]

        // Basic X-Factor
        [DataRow("1", 1, 'x', 0, 0)]
        [DataRow("1x", 1, 'x', 0, 0)]
        [DataRow("1.5", 1.5, 'x', 0, 0)]
        [DataRow("1.5x", 1.5, 'x', 0, 0)]
        [DataRow("2", 2, 'x', 0, 0)]
        [DataRow("2x", 2, 'x', 0, 0)]
        [DataRow("2.25", 2.25, 'x', 0, 0)]
        [DataRow("2.25x", 2.25, 'x', 0, 0)]
        [DataRow("12x", 12, 'x', 0, 0)] // vary large X-factor, without x it would default to 'w'

        // X-Factor with Width and maybe height
        [DataRow("1=45", 1, 'x', 45, 0)]
        [DataRow("1x=45", 1, 'x', 45, 0)]
        [DataRow("1.5=77", 1.5, 'x', 77, 0)]
        [DataRow("1.5x=77.9", 1.5, 'x', 78, 0)]
        [DataRow("1=45:33", 1, 'x', 45, 33)]
        [DataRow("1x=45:49", 1, 'x', 45, 49)]
        [DataRow("1.5=77:22.1", 1.5, 'x', 77, 22)]
        [DataRow("1.5x=77.9:22.9", 1.5, 'x', 78, 23)]

        [DataTestMethod]
        public void ParseSourcePart(string srcSet, double size, char sizeType = 'w', int? width = null, int height = 0) 
            => ParsePartAndCompareResult(srcSet, (float)size, sizeType, width, height);


        // Some invalid data - should basically result in ignored data
        [DataRow("1q", 0, 'd', 0, 0)]
        [DataRow("77vh", 0, 'd', 0, 0)]
        [DataRow("77vw", 0, 'w', 0, 0)] // this 'w' is picked up, because it's the last character
        [DataRow("100:100", 0, 'd', 0, 0)]
        [DataRow("99,9", 99, 'w', 99, 0)] // comma should never be used, but in production it will just work, because commas are split before

        [DataTestMethod]
        public void ParseFaultySourcePart(string srcSet, double size, char sizeType = 'w', int? width = null, int height = 0) 
            => ParsePartAndCompareResult(srcSet, (float)size, sizeType, width, height);


        /// <summary>
        /// Real test function - standalone to quickly also run a single test if it fails
        /// </summary>
        private static void ParsePartAndCompareResult(string srcSet, float size, char sizeType, int? width, int height)
        {
            var expected = new SrcSetPart(size, sizeType, width ?? (int)size, height);
            var result = SrcSetParser.ParsePart(srcSet);
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Size, result.Size, $"Sizes should match on '{srcSet}'");
            Assert.AreEqual(expected.SizeType, result.SizeType, $"Size Types should match on '{srcSet}'");
            Assert.AreEqual(expected.Width, result.Width, $"Widths should match on '{srcSet}'");
            Assert.AreEqual(expected.Height, result.Height, $"Heights should match on '{srcSet}'");
        }
    }
}
