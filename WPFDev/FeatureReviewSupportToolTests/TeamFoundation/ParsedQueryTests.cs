using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.TeamFoundation.VersionControl.Client;

using FeatureReviewSupportTool.TeamFoundation;

namespace FeatureReviewSupportToolTests.TeamFoundation
{
    [TestClass]
    public class ParsedQueryTests
    {
        [TestMethod]
        public void TestShouldParseOneWorkItem()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "5432" );
            Assert.AreEqual( 1, parsedQuery.WorkItemIds.Count, "Number of work items parsed." );
            CollectionAssert.Contains( parsedQuery.WorkItemIds, 5432 );
        }

        [TestMethod]
        public void TestShouldParseTwoWorkItems()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "5432, 4321" );
            Assert.AreEqual( 2, parsedQuery.WorkItemIds.Count, "Number of work items parsed." );
            CollectionAssert.Contains( parsedQuery.WorkItemIds, 5432 );
            CollectionAssert.Contains( parsedQuery.WorkItemIds, 4321 );
        }

        [TestMethod]
        public void TestShouldNotHaveVersionsForWorkItemNumbers()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "5432, 4321" );
            Assert.AreEqual( false, parsedQuery.HasVersions );
        }

        [TestMethod]
        public void TestShouldHaveVersionsForSingleLabel()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;Lone" );
            Assert.AreEqual( true, parsedQuery.HasVersions );
        }

        [TestMethod]
        public void TestShouldHaveVersionsForTwoLabels()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;Lone, Ltwo" );
            Assert.AreEqual( true, parsedQuery.HasVersions );
        }

        [TestMethod]
        public void TestShouldGetPath()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;Lone, Ltwo" );
            Assert.AreEqual( "$/Test", parsedQuery.ServerPath );
        }

        [TestMethod]
        public void TestShouldParseSingleLabelAsDiffToCurrent()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;LmyLabel" );
            Assert.AreEqual( "LmyLabel", parsedQuery.StartVersion.DisplayString, "start version" );
            Assert.AreEqual( VersionSpec.Latest, parsedQuery.EndVersion, "end version" );
        }

        [TestMethod]
        public void TestShouldParseLabelRangeWithCommas()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;LmyLabel, LotherLabel" );
            Assert.AreEqual( "LmyLabel", parsedQuery.StartVersion.DisplayString, "start version" );
            Assert.AreEqual( "LotherLabel", parsedQuery.EndVersion.DisplayString, "end version ('T' is latest)" );
        }

        [TestMethod]
        public void TestShouldParseLabelRangeWithDashes()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;LmyLabel    -    LotherLabel" );
            Assert.AreEqual( "LmyLabel", parsedQuery.StartVersion.DisplayString, "start version" );
            Assert.AreEqual( "LotherLabel", parsedQuery.EndVersion.DisplayString, "end version ('T' is latest)" );
        }

        [TestMethod]
        public void TestShouldParseLabelRangeWithDashesAndNoSpaces()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;LmyLabel-LotherLabel" );
            Assert.AreEqual( "LmyLabel", parsedQuery.StartVersion.DisplayString, "start version" );
            Assert.AreEqual( "LotherLabel", parsedQuery.EndVersion.DisplayString, "end version ('T' is latest)" );
        }

        [TestMethod]
        public void TestShouldParseLabelRangeWithJustWhitespace()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;LmyLabel   LotherLabel" );
            Assert.AreEqual( "LmyLabel", parsedQuery.StartVersion.DisplayString, "start version" );
            Assert.AreEqual( "LotherLabel", parsedQuery.EndVersion.DisplayString, "end version ('T' is latest)" );
        }

        [TestMethod]
        public void TestShouldParseLabelRangeWithTilde()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;LmyLabel~LotherLabel" );
            Assert.AreEqual( "LmyLabel", parsedQuery.StartVersion.DisplayString, "start version" );
            Assert.AreEqual( "LotherLabel", parsedQuery.EndVersion.DisplayString, "end version ('T' is latest)" );
        }

        [TestMethod]
        public void TestShouldHaveNoWorkItemNumbersForLabelSpec()
        {
            ParsedQuery parsedQuery = ParsedQuery.Parse( "$/Test;LmyLabel-LotherLabel" );
            Assert.IsNotNull( parsedQuery.WorkItemIds, "work item id collection should not be null." );
            Assert.AreEqual( 0, parsedQuery.WorkItemIds.Count );
        }
    }
}
