using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using uController.API.Search;

namespace uController.Tests
{
    [TestClass]
    public class PirateBayTests
    {
        private PirateBayProvider _provider;
        private SearchEventArgs _searchArgs;
        private ExtendedSearchEventArgs _extendedSearchArgs;

        [TestInitialize]
        public void TestInitialize()
        {
            _provider = new PirateBayProvider();
            _provider.SearchCompleted += new EventHandler<SearchEventArgs>(OnSearchCompleted);
            _provider.ExtendedSearchCompleted += new EventHandler<ExtendedSearchEventArgs>(OnExtendedSearchCompleted);
        }

        void OnSearchCompleted(object sender, SearchEventArgs e)
        {
            _searchArgs = e;
        }

        void OnExtendedSearchCompleted(object sender, ExtendedSearchEventArgs e)
        {
            _extendedSearchArgs = e;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _provider.SearchCompleted -= OnSearchCompleted;
            _provider = null;
            _searchArgs = null;
            _extendedSearchArgs = null;
        }

        [TestMethod]
        public void SearchIronMan()
        {
            _provider.Search("iron man");

            Assert.IsTrue(_searchArgs.Success, _searchArgs.ErrorMessage);
        }

        [TestMethod]
        public void SearchIronManGetInfo()
        {
            _provider.Search("iron man");

            Assert.IsTrue(_searchArgs.Success, _searchArgs.ErrorMessage);

            _provider.ExtendedSearch(_searchArgs.Urls.First());

            Assert.IsTrue(_extendedSearchArgs.Success, _extendedSearchArgs.ErrorMessage);
        }

        [TestMethod]
        public void TestBroken()
        {
            _provider.Search("nerd");

            Assert.IsTrue(_searchArgs.Success, _searchArgs.ErrorMessage);

            _provider.ExtendedSearch(_searchArgs.Urls.Skip(4).First());

            Assert.IsTrue(_extendedSearchArgs.Success, _extendedSearchArgs.ErrorMessage);
        }
    }
}
