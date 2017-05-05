﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PeanutButter.TestUtils.Generic;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;

namespace PeanutButter.RandomGenerators.Tests
{
    [TestFixture]
    public class TestEnumerableExtensions : TestBase
    {
        [Test]
        public void Randomize_OperatingOnEnumerableOfItems_ShouldGiveBackJumbledCollectionOfSameItems()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var results = new List<decimal>();
            for (var i = 0; i < RANDOM_TEST_CYCLES; i++)
            {
                var input = GetRandomCollection(() => GetRandomString(2, 10), 5, 10).ToArray();
                var jumbled = input.Randomize().ToArray();
                Assert.AreEqual(input.Length, jumbled.Length);
                var outOfPlace = 0;
                for (var j = 0; j < input.Length; j++)
                {
                    if (input[j] != jumbled[j])
                        outOfPlace++;
                }
                results.Add((decimal) outOfPlace / (decimal) input.Length);
            }

            //---------------Test Result -----------------------
            var average = results.Average();
            Assert.That(average, Is.GreaterThan(0.5));
        }

        [Test]
        public void Randomize_OperatingOnNull_ShouldReturnNull()
        {
            //---------------Set up test pack-------------------
            List<int> ints = null;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = ints.Randomize();

            //---------------Test Result -----------------------
            Assert.IsNull(result);
        }

        [Test]
        public void Randomize_OperatingOnEmptyCollection_ShouldReturnEmptyCollection()
        {
            //---------------Set up test pack-------------------
            var ints = new List<int>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = ints.Randomize();

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Randomize_ShouldReturnTheSameCollectionInADifferentOrder()
        {
            for (var i = 0; i < 10; i++) // it's entirely possible that the random output is the same, so let's try up to 10 times
            {
                //---------------Set up test pack-------------------
                var src = GetRandomCollection(() => GetRandomString(), 3);

                //---------------Assert Precondition----------------
                CollectionAssert.IsNotEmpty(src);

                //---------------Execute Test ----------------------
                var result = src.Randomize();

                //---------------Test Result -----------------------
                CollectionAssert.AreEquivalent(result, src);
                try
                {
                    CollectionAssert.AreNotEqual(result, src);
                    return;
                }
                catch (Exception)
                {
                    if (i < 9)
                        continue;
                    throw;
                }
            }
        }

        [Test]
        public void ShouldContainOneDeepEqualTo_OperatingOnEmptyCollection_ShouldThrow()
        {
            //--------------- Arrange -------------------
            var src = new string[] { };
            var seek = GetRandomString();

            //--------------- Assume ----------------

            //--------------- Act ----------------------
            Expect(() => src.ShouldContainOneDeepEqualTo(seek),
                Throws.Exception.InstanceOf<AssertionException>()
                    .With.Message.Contains("empty"));

            //--------------- Assert -----------------------
        }

        [Test]
        public void ShouldContainOneDeepEqualTo_OperatingOnCollectionNotContainingValue_ShouldThrow()
        {
            //--------------- Arrange -------------------
            var src = GetRandomCollection<string>(2);
            var seek = GetAnother(src);

            //--------------- Assume ----------------

            //--------------- Act ----------------------
            Expect(() => src.ShouldContainOneDeepEqualTo(seek),
                Throws.Exception.InstanceOf<AssertionException>()
                    .With.Message.Contains($"Expected to find \"{seek}\" in {Stringifier.Stringify(src)}"));

            //--------------- Assert -----------------------
        }

        [Test]
        public void ShouldContainOneDeepEqualTo_OperatingOnCollectionContainingOneMatchingValue_ShouldNotThrow()
        {
            //--------------- Arrange -------------------
            var src = GetRandomCollection<string>(4);
            var seek = GetRandomFrom(src);

            //--------------- Assume ----------------

            //--------------- Act ----------------------
            Expect(() => src.ShouldContainOneDeepEqualTo(seek),
                Throws.Nothing);

            //--------------- Assert -----------------------
        }

        [Test]
        public void ShouldContainOneDeepEqualTo_OperatingOnCollectionContainingTwoMatchingValues_ShouldThrow()
        {
            //--------------- Arrange -------------------
            var stuff = GetRandomCollection<string>(4);
            var seek = GetRandomFrom(stuff);
            var src = stuff.And(seek);

            //--------------- Assume ----------------

            //--------------- Act ----------------------
            Expect(() => src.ShouldContainOneDeepEqualTo(seek),
                Throws.Exception.InstanceOf<AssertionException>()
                    .With.Message.Contains("more than one match"));

            //--------------- Assert -----------------------
        }
        [Test]
        public void ShouldContainAtLeastOneDeepEqualTo_OperatingOnEmptyCollection_ShouldThrow()
        {
            //--------------- Arrange -------------------
            var src = new string[] { };
            var seek = GetRandomString();

            //--------------- Assume ----------------

            //--------------- Act ----------------------
            Expect(() => src.ShouldContainAtLeastOneDeepEqualTo(seek),
                Throws.Exception.InstanceOf<AssertionException>()
                    .With.Message.Contains("empty"));

            //--------------- Assert -----------------------
        }

        [Test]
        public void ShouldContainAtLeastOneDeepEqualTo_OperatingOnCollectionNotContainingValue_ShouldThrow()
        {
            //--------------- Arrange -------------------
            var src = GetRandomCollection<string>(2);
            var seek = GetAnother(src);

            //--------------- Assume ----------------

            //--------------- Act ----------------------
            Expect(() => src.ShouldContainAtLeastOneDeepEqualTo(seek),
                Throws.Exception.InstanceOf<AssertionException>()
                    .With.Message.Contains($"Expected to find \"{seek}\" in {Stringifier.Stringify(src)}"));

            //--------------- Assert -----------------------
        }

        [Test]
        public void ShouldContainAtLeastOneDeepEqualTo_OperatingOnCollectionContainingOneMatchingValue_ShouldNotThrow()
        {
            //--------------- Arrange -------------------
            var src = GetRandomCollection<string>(4);
            var seek = GetRandomFrom(src);

            //--------------- Assume ----------------

            //--------------- Act ----------------------
            Expect(() => src.ShouldContainAtLeastOneDeepEqualTo(seek),
                Throws.Nothing);

            //--------------- Assert -----------------------
        }

        [Test]
        public void ShouldContainAtLeastOneDeepEqualTo_OperatingOnCollectionContainingTwoMatchingValues_ShouldNotThrow()
        {
            //--------------- Arrange -------------------
            var stuff = GetRandomCollection<string>(4);
            var seek = GetRandomFrom(stuff);
            var src = stuff.And(seek);

            //--------------- Assume ----------------

            //--------------- Act ----------------------
            Expect(() => src.ShouldContainOneDeepEqualTo(seek),
                Throws.Exception.InstanceOf<AssertionException>()
                    .With.Message.Contains("more than one match"));

            //--------------- Assert -----------------------
        }



    }
}