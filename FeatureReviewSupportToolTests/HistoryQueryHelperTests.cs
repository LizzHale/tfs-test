using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sep.ConfigurationManagement.Vault.CodeReview;

namespace FeatureReviewSupportToolTests
{
    [TestClass]
    public class HistoryQueryHelperTests
    {
        [TestMethod]
        public void ShouldParseOneTaskId()
        {
            CollectionAssert.AreEqual( new string[] { "53" }, HistoryQueryHelper.GetTaskTags( "53" ) );
        }

        [TestMethod]
        public void ShouldParseTaskIdsWithSpaces()
        {
            CollectionAssert.AreEqual( new string[] { "53", "64" }, HistoryQueryHelper.GetTaskTags( "53, 64" ) );
        }

        [TestMethod]
        public void ShouldParseTaskIdsWithNoSpaces()
        {
            CollectionAssert.AreEqual( new string[] { "53", "64" }, HistoryQueryHelper.GetTaskTags( "53,64" ) );
        }

        [TestMethod]
        public void ShouldParseTaskIdsWithLetters()
        {
            CollectionAssert.AreEqual( new string[] { "I53", "A64" }, HistoryQueryHelper.GetTaskTags( "I53, A64" ) );
        }

        [TestMethod]
        public void ShouldNotMatchCommentsWithNoTags()
        {
            Assert.IsFalse( HistoryQueryHelper.IsChangeRelatedToTask( "Comment with no tags", "53" ) );
        }

        [TestMethod]
        public void ShouldNotMatchCommentsThatDoNotUseBrackets()
        {
            Assert.IsFalse( HistoryQueryHelper.IsChangeRelatedToTask( "53 comments", "53" ) );
        }

        [TestMethod]
        public void ShouldNotMatchCommentsWithDifferentIds()
        {
            Assert.IsFalse( HistoryQueryHelper.IsChangeRelatedToTask( "[64] blah", "53" ) );
        }

        [TestMethod]
        public void ShouldMatchCommentsWithTheSameId()
        {
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "[53] blah", "53" ) );
        }

        [TestMethod]
        public void ShouldMatchCommentsWithLettersInTaskId()
        {
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "[T53] blah", "T53" ) );
        }

        [TestMethod]
        public void ShouldMatchCommentsWithLettersInTaskIdIgnoringCase()
        {
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "[T53] blah", "t53" ) );
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "[t53] blah", "T53" ) );
        }

        [TestMethod]
        public void ShouldMatchCommentsWithOneMatchingIdInComment()
        {
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "[64,53] blah", "53" ) );
        }

        [TestMethod]
        public void ShouldMatchCommentsWithOneMatchingIdInCommentThatHasSpaces()
        {
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "[64, 53] blah", "53" ) );
        }

        [TestMethod]
        public void ShouldMatchCommentsWithOneMatchingIdInListOfChoices()
        {
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "[64] blah", "53", "64" ) );
        }

        [TestMethod]
        public void ShouldMatchCommentsWithMultipleBracketings()
        {
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "[53] [64] blah", "64" ) );
        }

        [TestMethod]
        public void ShouldMatchCommentsWithTagsAtTheEnd()
        {
            Assert.IsTrue( HistoryQueryHelper.IsChangeRelatedToTask( "blah [53]", "53" ) );
        }
    }
}
