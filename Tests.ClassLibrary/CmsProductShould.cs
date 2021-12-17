using ClassLibrary.Entities;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Tests.ClassLibrary
{
    public class CmsProductShould
    {
        [Fact]
        public void BeInstantiated()
        {
            var code = "some";
            var name = "name";
            var product = new CmsProduct(code, name);
            Assert.NotNull(product);
            Assert.Equal(code, product.Code);
            Assert.Equal(name, product.Name);
        }

        [Fact]
        public async Task BeCreatedFromAValidStream()
        {
            var code = "some";
            var name = "name";
            var stream = $"{{'code':'{code}', 'name':'{name}'}}".ToStream();
            var product = await CmsProduct.FromStreamAsync(stream);
            Assert.NotNull(product);
            Assert.Equal(code, product.Code);
            Assert.Equal(name, product.Name);
        }
    }

    public static class StringExtensions
    {
        public static Stream ToStream(this string text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(text);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
