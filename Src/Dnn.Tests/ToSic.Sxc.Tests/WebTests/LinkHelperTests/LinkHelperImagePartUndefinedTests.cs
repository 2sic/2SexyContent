﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Web;
using ToSic.Testing.Shared;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.WebTests.LinkHelperTests
{
    [TestClass()]
    public class LinkHelperImagePartUndefinedTests: EavTestBase
    {
        public ILinkHelper Link = LinkHelperResolver.LinkHelper();

        private void ImageVerifyUrlAreEqual(string testUrl)
        {
            AreEqual(testUrl, Link.Image(url: testUrl));
        }

        [TestMethod()]
        public void ImageNoUrlOrParamsTest()
        {
            ImageVerifyUrlAreEqual("");
        }

        [TestMethod()]
        public void ImageCommonUrlsTest()
        {
            ImageVerifyUrlAreEqual($"/");
            ImageVerifyUrlAreEqual($"/?a=1&b=2#fragment");
            ImageVerifyUrlAreEqual($"/page");
            ImageVerifyUrlAreEqual($"/page?a=1&b=2#fragment");
            ImageVerifyUrlAreEqual($"/file.ext");
            ImageVerifyUrlAreEqual($"/file.ext?a=1&b=2#fragment");
            ImageVerifyUrlAreEqual($"/folder/");
            ImageVerifyUrlAreEqual($"/folder/?a=1&b=2#fragment");
            ImageVerifyUrlAreEqual($"/folder/page");
            ImageVerifyUrlAreEqual($"/folder/page?a=1&b=2#fragment");
            ImageVerifyUrlAreEqual($"/folder/file.ext");
            ImageVerifyUrlAreEqual($"/folder/file.ext?a=1&b=2#fragment");
            ImageVerifyUrlAreEqual($"/folder/subfolder/");
            ImageVerifyUrlAreEqual($"/folder/subfolder/?a=1&b=2#fragment");
            ImageVerifyUrlAreEqual($"/folder/subfolder/page");
            ImageVerifyUrlAreEqual($"/folder/subfolder/page?a=1&b=2#fragment");
            ImageVerifyUrlAreEqual($"/folder/subfolder/file.ext");
            ImageVerifyUrlAreEqual($"/folder/subfolder/file.ext?a=1&b=2#fragment");
        }

        [TestMethod()]
        public void ImageUrlPathIsMissingTest()
        {
            ImageVerifyUrlAreEqual("?c=3");
            ImageVerifyUrlAreEqual("?param=c");
            ImageVerifyUrlAreEqual("?param=b&b=3&c=3");
            ImageVerifyUrlAreEqual("?#fragmentB");
            ImageVerifyUrlAreEqual("?param=c#fragmentB");
            ImageVerifyUrlAreEqual("#fragmentC");
        }

        [TestMethod()]
        public void ImageWithoutProtocolTest() // current behavior, potentially we can improve like in part "full"
        {
            AreEqual($"//unknown.2sxc.org/test", Link.Image(url: "//unknown.2sxc.org/test"));
        }

        [TestMethod()]
        public void ImageUrlWithTildeTest() // current behavior, potentially we can improve like in part "full"
        {
            AreEqual($"~", Link.Image(url: "~"));
            AreEqual($"~/", Link.Image(url: "~/"));
            AreEqual($"~/page", Link.Image(url: "~/page"));
            AreEqual($"~/file.ext", Link.Image(url: "~/file.ext"));
            AreEqual($"~/folder/", Link.Image(url: "~/folder/"));
        }

        [TestMethod()]
        public void ImageWithAbsoluteUrlTest()
        {
            ImageVerifyUrlAreEqual("https://unknown2.2sxc.org/");
            ImageVerifyUrlAreEqual("https://unknown2.2sxc.org/page");
            ImageVerifyUrlAreEqual("https://unknown2.2sxc.org/file.ext");
            ImageVerifyUrlAreEqual("https://unknown2.2sxc.org/folder/");
        }

        [TestMethod()]
        public void ImageWithInvalidUrlTest()
        {
            ImageVerifyUrlAreEqual("hello:there");
            ImageVerifyUrlAreEqual("file:593902");
            ImageVerifyUrlAreEqual("../file.ext");
            ImageVerifyUrlAreEqual("/sibling1/../sibling2/image.jpg");
        }

    }
}