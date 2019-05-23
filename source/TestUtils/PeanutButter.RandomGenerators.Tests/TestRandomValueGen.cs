using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GenericBuilderTestArtifactBuilders;
using GenericBuilderTestArtifactEntities;
using NExpect;
using NExpect.Interfaces;
using NExpect.MatcherLogic;
using NUnit.Framework;
using PeanutButter.Utils;
using static PeanutButter.RandomGenerators.RandomValueGen;
using static NExpect.Expectations;

// ReSharper disable AccessToDisposedClosure
// ReSharper disable PossibleMultipleEnumeration
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable InconsistentNaming
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace PeanutButter.RandomGenerators.Tests
{
    [TestFixture]
    public class TestRandomValueGen : TestBase
    {
        [TestCase(1, 100)]
        [TestCase(101, 250)]
        public void GetRandomInt_GivenRangeOfIntegers_ReturnsRandomIntWithinRange(int min, int max)
        {
            //---------------Set up test pack-------------------
            var ints = new List<int>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(() => ints.Add(GetRandomInt(min, max)), HIGH_RANDOM_TEST_CYCLES);

            //---------------Test Result -----------------------
            Expect(ints.All(i => i >= min))
                .To.Be.True(() => $"Numbers < min {min}: {ints.Where(i => i < min).JoinWith(",")}");
            Expect(ints.All(i => i <= max))
                .To.Be.True(() => $"Numbers > max {max}: {ints.Where(i => i > max).JoinWith(",")}");
            Expect(ints.Distinct().Count() > 1)
                .To.Be.True(() => $"Distinct count of numbers: {ints.Distinct().Count()}");
        }

        [Test]
        public void GetRandomInt_GivenRange_ShouldFindMinNumber()
        {
            // Arrange
            var min = GetRandomInt(1, 10);
            var max = GetRandomInt(11, 20);
            // Pre-assert
            // Act
            for (var i = 0; i < HIGH_RANDOM_TEST_CYCLES; i++)
            {
                var result = GetRandomInt(min, max);
                if (result == min)
                {
                    Assert.Pass();
                    return;
                }
            }

            // Assert
            Assert.Fail($"Unable to find {min} in range {min} - {max} over {HIGH_RANDOM_TEST_CYCLES} attempts");
        }
        
        [Test]
        public void GetRandomInt_GivenRange_ShouldFindMaxNumber()
        {
            // Arrange
            var min = GetRandomInt(1, 10);
            var max = GetRandomInt(11, 20);
            // Pre-assert
            // Act
            for (var i = 0; i < HIGH_RANDOM_TEST_CYCLES; i++)
            {
                var result = GetRandomInt(min, max);
                if (result == max)
                {
                    Assert.Pass();
                    return;
                }
            }

            // Assert
            Assert.Fail($"Unable to find {max} in range {min} - {max} over {HIGH_RANDOM_TEST_CYCLES} attempts");
        }

        [Test]
        public void GetRandomIntGeneric_GivenRangeOfIntegers_ReturnsRandomIntWithinRange()
        {
            //---------------Set up test pack-------------------
            var ints = new List<int>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(() => ints.Add(GetRandom<int>()));

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(ints);
            Assert.IsTrue(ints.All(i => i >= DefaultRanges.MIN_INT_VALUE));
            Assert.IsTrue(ints.All(i => i <= DefaultRanges.MAX_INT_VALUE));
        }

        [TestCase(1, 100)]
        [TestCase(101, 250)]
        public void GetRandomLong_GivenRangeOfIntegers_ReturnsRandomIntWithinRange(int min, int max)
        {
            //---------------Set up test pack-------------------
            var ints = new List<long>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(() => ints.Add(GetRandomLong(min, max)));

            //---------------Test Result -----------------------
            Assert.IsTrue(ints.All(i => i >= min));
            Assert.IsTrue(ints.All(i => i <= max));
            Assert.IsTrue(ints.Distinct().Count() > 1);
        }

        [Test]
        public void GetRandomLongGeneric_GivenRangeOfIntegers_ReturnsRandomIntWithinRange()
        {
            //---------------Set up test pack-------------------
            var ints = new List<long>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(() => ints.Add(GetRandom<long>()));

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(ints);
            Assert.IsTrue(ints.All(i => i >= DefaultRanges.MIN_LONG_VALUE));
            Assert.IsTrue(ints.All(i => i <= DefaultRanges.MAX_LONG_VALUE));
        }

        [TestCase(50, 100)]
        [TestCase(150, 400)]
        public void GetRandomString_GivenLengthLimits_ReturnsRandomStringsWithinLengthRange(
            int minLength,
            int maxLength)
        {
            //---------------Set up test pack-------------------
            var strings = new List<string>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(() => strings.Add(GetRandomString(minLength, maxLength)));

            //---------------Test Result -----------------------
            Assert.IsTrue(strings.All(s => s.Length >= minLength));
            Assert.IsTrue(strings.All(s => s.Length <= maxLength));
            Assert.IsTrue(strings.Distinct().Count() > 1);
        }

        [TestCase(50, 100)]
        [TestCase(150, 400)]
        public void GetRandomStringGeneric_GivenLengthLimits_ReturnsRandomStringsWithinLengthRange(
            int minLength,
            int maxLength)
        {
            //---------------Set up test pack-------------------
            var strings = new List<string>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(() => strings.Add(GetRandom<string>()));

            //---------------Test Result -----------------------
            Assert.IsTrue(strings.All(s => s.Length >= DefaultRanges.MINLENGTH_STRING));
            Assert.IsTrue(
                strings.All(
                    s => s.Length <=
                         DefaultRanges.MINLENGTH_STRING +
                         DefaultRanges.MINLENGTH_STRING));
            Assert.IsTrue(strings.Distinct().Count() > 1);
        }


        private enum TestEnum
        {
            One,
            Two,
            Three
        }

        [Test]
        public void GetRandomEnum_GENERIC_WhenGivenEnumTypeSpecifier_ShouldReturnRandomValueFromEnumSelection()
        {
            //---------------Set up test pack-------------------
            var results = new List<TestEnum>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            RunCycles(() => results.Add(GetRandomEnum<TestEnum>()));
            //---------------Test Result -----------------------
            var runs = results.Count;
            var onePercent = (100 * results.Count(i => i == TestEnum.One)) / runs;
            var twoPercent = (100 * results.Count(i => i == TestEnum.Two)) / runs;
            var threePercent = (100 * results.Count(i => i == TestEnum.Three)) / runs;

            var d1 = Math.Abs(twoPercent - onePercent);
            var d2 = Math.Abs(threePercent - twoPercent);
            var d3 = Math.Abs(threePercent - onePercent);

            Assert.That(d1, Is.LessThan(20));
            Assert.That(d2, Is.LessThan(20));
            Assert.That(d3, Is.LessThan(20));
        }

        [Test]
        public void GetRandom_GENERIC_WithEnumType_WhenGivenEnumTypeSpecifier_ShouldReturnRandomValueFromEnumSelection()
        {
            //---------------Set up test pack-------------------
            var results = new List<TestEnum>();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            RunCycles(() => results.Add(GetRandom<TestEnum>()));
            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(results);
        }

        [Test]
        public void GetRandomEnum_WITHTYPE_WhenGivenEnumTypeSpecifier_ShouldReturnRandomValueFromEnumSelection()
        {
            //---------------Set up test pack-------------------
            var results = new List<TestEnum>();
            var type = typeof(TestEnum);
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------

            RunCycles(() => results.Add((TestEnum) GetRandomEnum(type)));
            //---------------Test Result -----------------------
            var runs = results.Count;
            var onePercent = (100 * results.Count(i => i == TestEnum.One)) / runs;
            var twoPercent = (100 * results.Count(i => i == TestEnum.Two)) / runs;
            var threePercent = (100 * results.Count(i => i == TestEnum.Three)) / runs;

            var d1 = Math.Abs(twoPercent - onePercent);
            var d2 = Math.Abs(threePercent - twoPercent);
            var d3 = Math.Abs(threePercent - onePercent);

            Assert.That(d1, Is.LessThan(20));
            Assert.That(d2, Is.LessThan(20));
            Assert.That(d3, Is.LessThan(20));
        }

        [Test]
        public void GetRandomFrom_WhenGivenIEnumerableOfItems_ShouldReturnARandomItemFromTheCollection()
        {
            //---------------Set up test pack-------------------
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var items = new[] {o1, o2, o3};
            var results = new List<object>();
            const int runs = NORMAL_RANDOM_TEST_CYCLES;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < runs; i++)
            {
                results.Add(GetRandomFrom(items));
            }

            //---------------Test Result -----------------------
            Assert.IsTrue(results.All(r => items.Contains(r)));
            Assert.IsTrue(items.All(i => results.Any(r => r == i)));
        }

        [Test]
        public void GetRandomSelectionFrom_WhenGivenIEnumerableOfItems_ShouldReturnARandomCollectionFromTheCollection()
        {
            //---------------Set up test pack-------------------
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var items = new[] {o1, o2, o3};
            var results = new List<IEnumerable<object>>();
            const int runs = NORMAL_RANDOM_TEST_CYCLES;
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < runs; i++)
            {
                results.Add(GetRandomSelectionFrom(items));
            }

            //---------------Test Result -----------------------
            var flattened = results.SelectMany(
                r =>
                {
                    var collections = r as object[] ?? r.ToArray();
                    return collections;
                });
            Assert.IsTrue(flattened.All(f => items.Contains(f)));
            var averageCount = results.Select(r => r.Count()).Average();
            Assert.That(averageCount, Is.GreaterThan(1));
            Assert.That(averageCount, Is.LessThan(items.Length));
        }

        [Test]
        public void GetRandomSelectionFrom_ShouldNotRepeatItems()
        {
            //---------------Set up test pack-------------------
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var items = new[] {o1, o2, o3};
            const int runs = NORMAL_RANDOM_TEST_CYCLES;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < runs; i++)
            {
                var result = GetRandomSelectionFrom(items);
                //---------------Test Result -----------------------
                CollectionAssert.AreEqual(result, result.Distinct());
            }
        }

        [Test]
        public void GetRandomSelectionFrom_ShouldProvideCollectionWithinRequiredRangeOfSize()
        {
            //---------------Set up test pack-------------------
            var o1 = new object();
            var o2 = new object();
            var o3 = new object();
            var o4 = new object();
            var o5 = new object();
            var o6 = new object();
            var items = new[] {o1, o2, o3, o4, o5, o6};
            var min = GetRandomInt(1, 3);
            var max = GetRandomInt(3, items.Length);
            const int runs = NORMAL_RANDOM_TEST_CYCLES;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < runs; i++)
            {
                var result = GetRandomSelectionFrom(items, min, max);
                //---------------Test Result -----------------------
                Assert.That(result.Count(), Is.GreaterThanOrEqualTo(min));
                Assert.That(result.Count(), Is.LessThanOrEqualTo(max));
            }
        }

        [Test]
        public void GetRandomCollection_GivenFactoryFunction_ShouldInvokeItToCreateItems()
        {
            //---------------Set up test pack-------------------
            const int runs = NORMAL_RANDOM_TEST_CYCLES;
            var generatedValues = new List<int>();
            Func<int> factory = () =>
            {
                var thisValue = GetRandomInt();
                generatedValues.Add(thisValue);
                return thisValue;
            };
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < runs; i++)
            {
                var result = GetRandomCollection(factory);
                //---------------Test Result -----------------------
                CollectionAssert.AreEqual(generatedValues, result);
                generatedValues.Clear();
            }
        }

        [Test]
        public void GetRandomCollection_GenericInvoke_ShouldUseNinjaSuperPowersToCreateCollection()
        {
            //---------------Set up test pack-------------------
            var minItems = GetRandomInt(5);
            var maxItems = GetRandomInt(11, 20);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetRandomCollection<SomePOCO>(minItems, maxItems);

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.IsTrue(result.All(r => r != null));
            Assert.IsTrue(result.All(r => r.GetType() == typeof(SomePOCO)));
            VarianceAssert.IsVariant<SomePOCO, int>(result, "Id");
            VarianceAssert.IsVariant<SomePOCO, string>(result, "Name");
            VarianceAssert.IsVariant<SomePOCO, DateTime>(result, "Date");
        }

        [Test]
        public void GetRandomArray_GivenFactoryFunction_ShouldInvokeItToCreateItems()
        {
            //---------------Set up test pack-------------------
            const int runs = NORMAL_RANDOM_TEST_CYCLES;
            var generatedValues = new List<int>();
            Func<int> factory = () =>
            {
                var thisValue = GetRandomInt();
                generatedValues.Add(thisValue);
                return thisValue;
            };
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < runs; i++)
            {
                var result = GetRandomArray(factory);
                //---------------Test Result -----------------------
                CollectionAssert.AreEqual(generatedValues, result);
                generatedValues.Clear();
            }
        }

        [Test]
        public void GetRandomArray_GenericInvoke_ShouldUseNinjaSuperPowersToCreateArray()
        {
            //---------------Set up test pack-------------------
            var minItems = GetRandomInt(5);
            var maxItems = GetRandomInt(11, 20);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetRandomArray<SomePOCO>(minItems, maxItems);

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            CollectionAssert.IsNotEmpty(result);
            Assert.IsTrue(result.All(r => r != null));
            Assert.IsTrue(result.All(r => r.GetType() == typeof(SomePOCO)));
            VarianceAssert.IsVariant<SomePOCO, int>(result, "Id");
            VarianceAssert.IsVariant<SomePOCO, string>(result, "Name");
            VarianceAssert.IsVariant<SomePOCO, DateTime>(result, "Date");
        }


        [TestFixture]
        public class GetRandomDates : TestRandomValueGen
        {
            [TestCase(1984, 4, 4, 2001, 1, 1)]
            [TestCase(1914, 4, 4, 2011, 1, 1)]
            [TestCase(2001, 4, 4, 2001, 1, 1)]
            public void GetRandomDate_GivenDateOnlyIsTrue_ShouldReturnDateTimeWithNoTimeComponent(
                int minYear,
                int minMonth,
                int minDay,
                int maxYear,
                int maxMonth,
                int maxDay)
            {
                //---------------Set up test pack-------------------
                var results = new List<DateTime>();
                var range = new DateTimeRangeContainer(minYear, minMonth, minDay, maxYear, maxMonth, maxDay);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                RunCycles(() => results.Add(GetRandomDate(range.From, range.To, true)));

                //---------------Test Result -----------------------
                Assert.AreEqual(NORMAL_RANDOM_TEST_CYCLES, results.Count);
                Assert.IsTrue(results.All(range.InRange), "One or more generated value is out of range");
                Assert.IsTrue(results.All(d => d.Hour == 0), "Hours are not all zeroed");
                Assert.IsTrue(results.All(d => d.Minute == 0), "Minutes are not all zeroed");
                Assert.IsTrue(results.All(d => d.Second == 0), "Seconds are not all zeroed");
                Assert.IsTrue(results.All(d => d.Millisecond == 0), "Seconds are not all zeroed");
            }

            [Test]
            public void GetRandomDate_GivenMinTimeOrMaxTime_AndDateOnlyIsTrue_ShouldIgnoreTheGivenTimes()
            {
                //---------------Set up test pack-------------------
                var ticksInOneDay = TimeSpan.FromDays(1).Ticks;
                var minTicks = GetRandomLong(1, ticksInOneDay - 1);
                var maxTicks = GetRandomLong(minTicks, ticksInOneDay - 1);
                var minTime = GetRandomDate().StartOfDay().AddTicks(minTicks);
                var maxTime = GetRandomDate().StartOfDay().AddTicks(maxTicks);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = GetRandomDate(dateOnly: true, minTime: minTime, maxTime: maxTime);

                //---------------Test Result -----------------------
                Assert.AreEqual(result.StartOfDay(), result);
            }

            [TestCase(1984, 4, 4, 2001, 1, 1)]
            [TestCase(1914, 4, 4, 2011, 1, 1)]
            [TestCase(2001, 4, 4, 2001, 1, 1)]
            public void GetRandomDate_ShouldReturnDatesWithinRange(
                int minYear,
                int minMonth,
                int minDay,
                int maxYear,
                int maxMonth,
                int maxDay)
            {
                //---------------Set up test pack-------------------
                var results = new List<DateTime>();
                var range = new DateTimeRangeContainer(minYear, minMonth, minDay, maxYear, maxMonth, maxDay);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                RunCycles(() => results.Add(GetRandomDate(range.From, range.To)));

                //---------------Test Result -----------------------
                Assert.AreEqual(NORMAL_RANDOM_TEST_CYCLES, results.Count);
                Assert.IsTrue(results.All(d => d >= range.From), "One or more results is less than the minimum date");
                Assert.IsTrue(results.All(d => d <= range.To), "One or more results is greater than the maximum date");
                Assert.IsTrue(results.All(d => d.Microseconds() == 0), "Microseconds should be zeroed on random dates");
            }

            [Test]
            public void GetRandomTimeOn_GivenDate_ShouldReturnRandomDateTimeOnThatDay()
            {
                //---------------Set up test pack-------------------
                var theDate = GetRandomDate();
                var min = new DateTime(theDate.Year, theDate.Month, theDate.Day, 0, 0, 0);
                var max = new DateTime(theDate.Year, theDate.Month, theDate.Day, 0, 0, 0);
                max = max.AddDays(1).AddMilliseconds(-1);
                var results = new List<DateTime>();

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                RunCycles(() => results.Add(GetRandomTimeOn(theDate)));

                //---------------Test Result -----------------------
                Assert.IsTrue(results.All(d => d >= min));
                Assert.IsTrue(results.All(d => d <= max));
            }

            [Test]
            public void GetRandomDate_GivenMinTime_ShouldProduceRandomDatesWithTimesGreaterOrEqual()
            {
                //---------------Set up test pack-------------------
                var minTime = new DateTime(1900, 1, 1, GetRandomInt(0, 12), GetRandomInt(0, 59), GetRandomInt(0, 59));
                var maxTime = new DateTime(minTime.Year, minTime.Month, minTime.Day, 0, 0, 0);
                maxTime = maxTime.AddDays(1).AddMilliseconds(-1);
                var results = new List<DateTime>();

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                RunCycles(() => results.Add(GetRandomDate(minTime: minTime, maxTime: maxTime)));

                //---------------Test Result -----------------------
                var outOfRangeLeft = results.Where(d => d.Ticks < minTime.Ticks).ToArray();
                var outOfRangeRight = results.Where(d => d.Ticks < maxTime.Ticks).ToArray();
                Assert.IsFalse(
                    outOfRangeLeft.Any() && outOfRangeRight.Any(),
                    GetErrorHelpFor(outOfRangeLeft, outOfRangeRight, minTime, maxTime));
            }

            [Test]
            [Repeat(NORMAL_RANDOM_TEST_CYCLES)]
            public void GetRandomDate_GivenMinDateOnly_ShouldProduceRandomDatesWithin30YearPeriod()
            {
                //---------------Set up test pack-------------------
                var minDate = new DateTime(GetRandomInt(1, 3000), GetRandomInt(1, 12), GetRandomInt(1, 28));
                var maxDate = minDate.AddYears(30);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = GetRandomDate(minDate: minDate);

                //---------------Test Result -----------------------
                Assert.That(
                    result,
                    Is.GreaterThanOrEqualTo(minDate)
                        .And.LessThanOrEqualTo(maxDate));
            }

            [Test]
            [Repeat(NORMAL_RANDOM_TEST_CYCLES)]
            public void GetRandomDate_GivenMaxDateOnly_ShouldProduceRandomDatesWithin30YearPeriod()
            {
                //---------------Set up test pack-------------------
                var maxDate = new DateTime(GetRandomInt(31, 3000), GetRandomInt(1, 12), GetRandomInt(1, 28));
                var minDate = maxDate.AddYears(-30);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = GetRandomDate(maxDate: maxDate);

                //---------------Test Result -----------------------
                Assert.That(
                    result,
                    Is.GreaterThanOrEqualTo(minDate)
                        .And.LessThanOrEqualTo(maxDate));
            }

            [Test]
            [Repeat(NORMAL_RANDOM_TEST_CYCLES)]
            public void
                GetRandomDate_GivenMinAndMaxTimeWithFractionsOfSeconds_ShouldTruncateTimesToMilliseconds_AndProduceRandomDateWithTimeBetweenMinAndMax()
            {
                //---------------Set up test pack-------------------
                var r = GetRandomDate();
                var baseDateTime = new DateTime(r.Year, r.Month, r.Day, r.Hour, r.Minute, r.Second);
                var ticksInOneSecond = TimeSpan.FromSeconds(1).Ticks;
                var minTicks = GetRandomLong(1, ticksInOneSecond - 1);
                var maxTicks = GetRandomLong(minTicks, ticksInOneSecond - 1);
                var minTime = baseDateTime.AddTicks(minTicks);
                var maxTime = baseDateTime.AddTicks(maxTicks);
                var expectedMinTime = minTime.TruncateMicroseconds();
                var expectedMaxTime = maxTime.TruncateMicroseconds();

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = GetRandomDate(minTime: minTime, maxTime: maxTime);

                //---------------Test Result -----------------------
                Assert.That(
                    result.TimeOfDay,
                    Is.GreaterThanOrEqualTo(expectedMinTime.TimeOfDay)
                        .And.LessThanOrEqualTo(expectedMaxTime.TimeOfDay));
            }

            [Test]
            public void GetRandomDate_GivenMaxTime_ShouldProduceRandomDatesWithTimesLessThanOrEqual()
            {
                //---------------Set up test pack-------------------
                var maxTime = new DateTime(1900, 1, 1, GetRandomInt(12, 23), GetRandomInt(0, 59), GetRandomInt(0, 59));
                var results = new List<DateTime>();

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                RunCycles(() => results.Add(GetRandomDate(maxTime: maxTime)));

                //---------------Test Result -----------------------
                var outOfRange = results
                    .Where(d => d.MillisecondsSinceStartOfDay() > maxTime.MillisecondsSinceStartOfDay())
                    .ToArray();
                Assert.IsFalse(
                    outOfRange.Any(),
                    $"One or more results had a time that was too late for {maxTime}.{Environment.NewLine}{Print(outOfRange)}");
            }

            private string Print(DateTime[] outOfRange)
            {
                return string.Join(Environment.NewLine, outOfRange);
            }

            [Test]
            public void GetRandomDate_GivenMinDateTimeAndMaxDateTime_WhenDateOnlySpecified_ShouldReturnDateWithinRange()
            {
                //---------------Set up test pack-------------------
                var minDate = new DateTime(2011, 1, 1, 23, 30, 0);
                var maxDate = new DateTime(2011, 1, 2, 00, 30, 0);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                RunCycles(
                    () =>
                    {
                        var result = GetRandomDate(minDate, maxDate, true);
                        Assert.AreEqual(new DateTime(2011, 1, 2, 0, 0, 0), result);
                    });

                //---------------Test Result -----------------------
            }

            [Test]
            public void
                GetRandomDate_GivenMinDateTimeAndMaxDateTime_WhenDateOnlySpecified_AndMinMaxOnSameDay_ShouldGiveThatDay()
            {
                //---------------Set up test pack-------------------
                var minDate = new DateTime(2011, 1, 1, 12, 00, 0);
                var maxDate = new DateTime(2011, 1, 1, 12, 30, 0);

                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                RunCycles(
                    () =>
                    {
                        var result = GetRandomDate(minDate, maxDate, true);
                        Assert.AreEqual(new DateTime(2011, 1, 1, 0, 0, 0), result);
                    });

                //---------------Test Result -----------------------
            }
        }

        [TestFixture]
        public class GetRandomTimespans
        {
            [Test]
            public void WhenContextIsMilliseconds_ShouldReturnSecondRange()
            {
                // Arrange
                // Pre-assert
                // Act
                var result = GetRandomTimeSpan(1, 10, TimeSpanContexts.Milliseconds);
                // Assert
                Expect(result.Ticks).To.Be
                    .Greater.Than(TimeSpan.FromMilliseconds(1).Ticks - 1)
                    .And
                    .Less.Than(TimeSpan.FromMilliseconds(10).Ticks + 1);
            }

            [Test]
            public void WhenContextIsMinutes_ShouldReturnSecondRange()
            {
                // Arrange
                // Pre-assert
                // Act
                var result = GetRandomTimeSpan(1, 10, TimeSpanContexts.Minutes);
                // Assert
                Expect(result.Ticks).To.Be
                    .Greater.Than(TimeSpan.FromMinutes(1).Ticks - 1)
                    .And
                    .Less.Than(TimeSpan.FromMinutes(10).Ticks + 1);
            }
            
            [Test]
            public void WhenContextIsHours_ShouldReturnSecondRange()
            {
                // Arrange
                // Pre-assert
                // Act
                var result = GetRandomTimeSpan(1, 10, TimeSpanContexts.Hours);
                // Assert
                Expect(result.Ticks).To.Be
                    .Greater.Than(TimeSpan.FromHours(1).Ticks - 1)
                    .And
                    .Less.Than(TimeSpan.FromHours(10).Ticks + 1);
            }
                        
            [Test]
            public void WhenContextIsDays_ShouldReturnSecondRange()
            {
                // Arrange
                // Pre-assert
                // Act
                var result = GetRandomTimeSpan(1, 10, TimeSpanContexts.Days);
                // Assert
                Expect(result.Ticks).To.Be
                    .Greater.Than(TimeSpan.FromDays(1).Ticks - 1)
                    .And
                    .Less.Than(TimeSpan.FromDays(10).Ticks + 1);
            }
                        
            [Test]
            public void WhenContextIsSeconds_ShouldReturnSecondRange()
            {
                // Arrange
                // Pre-assert
                // Act
                var result = GetRandomTimeSpan(1, 10, TimeSpanContexts.Seconds);
                // Assert
                Expect(result.Ticks).To.Be
                    .Greater.Than(TimeSpan.FromSeconds(1).Ticks - 1)
                    .And
                    .Less.Than(TimeSpan.FromSeconds(10).Ticks + 1);
            }
            
        }


        [Test]
        public void GetRandomDateRange_GivenNoArguments_ShouldReturnRandomDateRange()
        {
            //---------------Set up test pack-------------------
            var allResults = new List<DateRange>();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(
                () =>
                {
                    var thisResult = GetRandomDateRange();
                    Assert.IsNotNull(thisResult);
                    Assert.That(thisResult.From, Is.LessThanOrEqualTo(thisResult.To));
                    allResults.Add(thisResult);
                });

            //---------------Test Result -----------------------
            var froms = allResults.Select(o => o.From);
            var tos = allResults.Select(o => o.To);
            var deltas = allResults.Select(o => o.To - o.From);
            VarianceAssert.IsVariant(froms);
            VarianceAssert.IsVariant(tos);
            VarianceAssert.IsVariant(deltas);
        }

        [Test]
        public void GetRandomDateRange_GivenMinDate_ShouldReturnRangeWithBothDatesGreaterThanMinDate()
        {
            //---------------Set up test pack-------------------
            var minDate = GetRandomDate();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetRandomDateRange(minDate);

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            Assert.That(result.From, Is.GreaterThanOrEqualTo(minDate));
            Assert.That(result.To, Is.GreaterThanOrEqualTo(minDate));
        }

        [Test]
        public void GetRandomDateRange_GivenMinDateAndMaxDate_ShouldReturnRangeWithinMinAndMaxRange()
        {
            //---------------Set up test pack-------------------
            var minDate = GetRandomDate();
            var maxDate = minDate.AddDays(GetRandomInt(1, 12));


            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetRandomDateRange(minDate, maxDate);

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            Assert.That(result.From, Is.GreaterThanOrEqualTo(minDate));
            Assert.That(result.To, Is.GreaterThanOrEqualTo(minDate));
            Assert.That(result.From, Is.LessThanOrEqualTo(maxDate));
            Assert.That(result.To, Is.LessThanOrEqualTo(maxDate));
        }

        [Test]
        public void
            GetRandomDateRange_GivenMinDateAndMaxDateAndDateOnlyTrue_ShouldReturnRangeWithinMinAndMaxRangeWithNoTimes()
        {
            //---------------Set up test pack-------------------
            var minDate = GetRandomDate();
            var maxDate = minDate.AddDays(GetRandomInt(1, 12));


            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetRandomDateRange(minDate, maxDate, true);

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            Assert.That(result.From, Is.GreaterThanOrEqualTo(minDate));
            Assert.That(result.To, Is.GreaterThanOrEqualTo(minDate));
            Assert.That(result.From, Is.LessThanOrEqualTo(maxDate));
            Assert.That(result.To, Is.LessThanOrEqualTo(maxDate));
            Assert.AreEqual(result.From.StartOfDay(), result.From);
            Assert.AreEqual(result.To.StartOfDay(), result.To);
        }

        [Test]
        public void GetRandomDateRange_GivenMinTime_ShouldEnsureBothDatesAreAfterThatTime()
        {
            //---------------Set up test pack-------------------
            var minTime = GetRandomDate();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(
                () =>
                {
                    var result = GetRandomDateRange(minTime: minTime);

                    //---------------Test Result -----------------------
                    Assert.That(result.From.TimeOfDay, Is.GreaterThanOrEqualTo(minTime.TimeOfDay));
                    Assert.That(result.To.TimeOfDay, Is.GreaterThanOrEqualTo(minTime.TimeOfDay));
                });
        }

        [Test]
        public void GetRandomDateRange_GivenMaxTime_ShouldEnsureBothDatesAreBeforeThatTime()
        {
            //---------------Set up test pack-------------------
            var maxTime = GetRandomDate();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(
                () =>
                {
                    var result = GetRandomDateRange(maxTime: maxTime);

                    //---------------Test Result -----------------------
                    Assert.That(result.From.TimeOfDay, Is.LessThanOrEqualTo(maxTime.TimeOfDay));
                    Assert.That(result.To.TimeOfDay, Is.LessThanOrEqualTo(maxTime.TimeOfDay));
                });
        }

        [Test]
        public void GetRandomDateRange_GivenDateKind_ShouldReturnBothDatesWithThatKind()
        {
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var expected = GetRandom<DateTimeKind>();

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = GetRandomDateRange(expected);

                    //---------------Test Result -----------------------
                    Assert.AreEqual(expected, result.From.Kind);
                    Assert.AreEqual(expected, result.To.Kind);
                });
        }

        [Test]
        public void GetRandomDateRange_GivenNoDateKind_ShouldSetLocal()
        {
            //---------------Set up test pack-------------------
            var expected = DateTimeKind.Local;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetRandomDateRange();

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result.From.Kind);
            Assert.AreEqual(expected, result.To.Kind);
        }

        [Test]
        public void
            GetRandomCollection_GivenGeneratorFunctionAndBoundaries_ShouldReturnListOfRandomSizeContainingOutputOfGeneratorPerItem()
        {
            //---------------Set up test pack-------------------
            const int runs = NORMAL_RANDOM_TEST_CYCLES;

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < runs; i++)
            {
                var min = GetRandomInt(10, 100);
                var max = GetRandomInt(10, 100);
                if (min > max)
                {
                    var swap = min;
                    min = max;
                    max = swap;
                }

                var fill = GetRandomInt(1, 1024);
                var result = GetRandomCollection(() => fill, min, max);


                //---------------Test Result -----------------------
                Assert.That(result.Count(), Is.GreaterThanOrEqualTo(min));
                Assert.That(result.Count(), Is.LessThanOrEqualTo(max));
                Assert.IsTrue(result.All(item => item == fill));
            }
        }

        [Test]
        public void GetRandomCollection_WhenMinEqualsMax_ShouldReturnExactlyThatSize()
        {
            //---------------Set up test pack-------------------
            int max;
            var min = max = GetRandomInt();

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetRandomCollection(() => GetRandomInt(), min, max);

            //---------------Test Result -----------------------
            Assert.AreEqual(min, result.Count());
        }


        [Test]
        public void GetRandomAlphaNumericString_ShouldProduceRandomStringWithOnlyAlphaNumericCharacters()
        {
            var allResults = new List<Tuple<string, int, int>>();
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var minLength = GetRandomInt(1, 50);
                    var maxLength = GetRandomInt(minLength, minLength + 50);

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = GetRandomAlphaNumericString(minLength, maxLength);

                    allResults.Add(Tuple.Create(result, minLength, maxLength));
                });
            //---------------Test Result -----------------------
            CollectionAssert.IsNotEmpty(allResults);
            // collisions are possible, but should occur < 1%
            var total = allResults.Count;
            var unique = allResults.Select(o => o.Item1).Distinct().Count();
            var delta = (total - unique) / (decimal) total;
            Assert.That(delta, Is.LessThan(1));

            var tooShort = allResults.Where(r => r.Item1.Length < r.Item2);
            var tooLong = allResults.Where(r => r.Item1.Length > r.Item3);
            var alphaNumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var invalidCharacters = allResults.Where(r => r.Item1.Any(c => !alphaNumericChars.Contains(c)));
            Assert.IsFalse(
                tooShort.Any() && tooLong.Any() && invalidCharacters.Any(),
                BuildErrorMessageFor(tooShort, tooLong, invalidCharacters));
        }

        [Test]
        public void GetRandomNonAlphaNumericString_ShouldProduceNonAlphaNumericStrings()
        {
            // Arrange
            var results = new List<string>();
            // Pre-assert
            // Act
            RunCycles(() => results.Add(GetRandomNonAlphaNumericString(1)));
            // Assert
            Expect(results.None(r => r.IsAlphanumeric()));
        }

        [Test]
        public void GetRandomAlphaString_ShouldProduceRandomStringWithOnlyAlphaCharacters()
        {
            var allResults = new List<Tuple<string, int, int>>();
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var minLength = GetRandomInt(1, 50);
                    var maxLength = GetRandomInt(minLength, minLength + 50);

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = GetRandomAlphaString(minLength, maxLength);

                    allResults.Add(Tuple.Create(result, minLength, maxLength));
                });
            //---------------Test Result -----------------------
            CollectionAssert.IsNotEmpty(allResults);
            // collisions are possible, but should occur < 1%
            var total = allResults.Count;
            var unique = allResults.Select(o => o.Item1).Distinct().Count();
            var delta = (total - unique) / (decimal) total;
            Assert.That(delta, Is.LessThan(1));

            var tooShort = allResults.Where(r => r.Item1.Length < r.Item2);
            var tooLong = allResults.Where(r => r.Item1.Length > r.Item3);
            var alphaNumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var invalidCharacters = allResults.Where(r => r.Item1.Any(c => !alphaNumericChars.Contains(c)));
            Assert.IsFalse(
                tooShort.Any() && tooLong.Any() && invalidCharacters.Any(),
                BuildErrorMessageFor(tooShort, tooLong, invalidCharacters));
        }

        [Test]
        public void GetRandomNumericString_ShouldProduceRandomStringWithOnlyNumericCharacters()
        {
            var allResults = new List<Tuple<string, int, int>>();
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var minLength = GetRandomInt(1, 50);
                    var maxLength = GetRandomInt(minLength, minLength + 50);

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = GetRandomNumericString(minLength, maxLength);

                    allResults.Add(Tuple.Create(result, minLength, maxLength));
                });
            //---------------Test Result -----------------------
            CollectionAssert.IsNotEmpty(allResults);
            // collisions are possible, but should occur < 1%
            var total = allResults.Count;
            var unique = allResults.Select(o => o.Item1).Distinct().Count();
            var delta = (total - unique) / (decimal) total;
            Assert.That(delta, Is.LessThan(1));

            var tooShort = allResults.Where(r => r.Item1.Length < r.Item2);
            var tooLong = allResults.Where(r => r.Item1.Length > r.Item3);
            var alphaNumericChars = "1234567890";
            var invalidCharacters = allResults.Where(r => r.Item1.Any(c => !alphaNumericChars.Contains(c)));
            Assert.IsFalse(
                tooShort.Any() && tooLong.Any() && invalidCharacters.Any(),
                BuildErrorMessageFor(tooShort, tooLong, invalidCharacters));
        }

        [Test]
        public void GetAnother_GivenOriginalValueAndGenerator_WhenCanGenerateNewValue_ShouldReturnANewValue()
        {
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var notThis = GetRandomString(1, 1);

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = GetAnother(notThis, () => GetRandomString(1, 1));

                    //---------------Test Result -----------------------
                    Assert.AreNotEqual(notThis, result);
                });
        }

        [Test]
        public void GetAnother_GivenOriginalValueAndNoGenerator_WhenCanGenerateNewValue_ShouldReturnANewValue()
        {
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var notThis = GetRandomString(1, 1);

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = GetAnother(notThis);

                    //---------------Test Result -----------------------
                    Assert.AreNotEqual(notThis, result);
                });
        }

        [Test]
        public void GetAnother_GivenOriginalValueAndGenerator_WhenCannotGenerateNewValue_ShouldThrow()
        {
            //---------------Set up test pack-------------------
            var notThis = GetRandomString(1, 1);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.Throws<CannotGetAnotherDifferentRandomValueException<string>>(
                () => GetAnother(
                    notThis,
                    () => notThis));

            //---------------Test Result -----------------------
        }

        [Test]
        public void
            GetAnother_GivenOriginalValueAndGenerator_WhenCannotGenerateNewValueBecauseOfComparisonFunc_ShouldThrow()
        {
            //---------------Set up test pack-------------------
            var notThis = GetRandomString(1, 1);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.Throws<CannotGetAnotherDifferentRandomValueException<string>>(
                () => GetAnother(
                    notThis,
                    () => GetRandomString(),
                    (left, right) => true));

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetAnother_GivenOriginalValueCollectionAndGenerator_WhenCanGenerateNewValue_ShouldReturnThatValue()
        {
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var notThis = "abcdefghijklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVW".ToCharArray()
                        .Select(c => c.ToString());

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = GetAnother(notThis, () => GetRandomString(1, 1));

                    //---------------Test Result -----------------------
                    Assert.IsFalse(notThis.Any(i => i == result));
                });
        }

        [Test]
        public void
            GetAnother_GivenOriginalValueCollectionAndNoGenerator_WhenCanGenerateNewValue_ShouldReturnThatValue()
        {
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var notThis = "abcdefghijklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVW".ToCharArray()
                        .Select(c => c.ToString());

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = GetAnother(notThis);

                    //---------------Test Result -----------------------
                    Assert.IsFalse(notThis.Any(i => i == result));
                });
        }


        [Test]
        public void
            GetAnother_GivenOriginalValueCollectionAndGenerator_WhenCannotGenerateNewValueBecauseOfComparisonFunc_ShouldThrow()
        {
            //---------------Set up test pack-------------------
            var notAnyOfThese = GetRandomCollection(() => GetRandomString(), 2);

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.Throws<CannotGetAnotherDifferentRandomValueException<IEnumerable<string>>>(
                () => GetAnother(
                    notAnyOfThese,
                    () => GetRandomString(),
                    (left, right) => true));

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetAnother_GivenNullValue_ShouldReturnValueFromGenerator()
        {
            //---------------Set up test pack-------------------
            var strings = new Stack<string>();
            var expected = GetRandomString();
            var unexpected = GetAnother(expected, () => GetRandomString());
            strings.Push(unexpected);
            strings.Push(expected);
            strings.Push(null);


            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetAnother((string) null, () => strings.Pop());

            //---------------Test Result -----------------------
            Assert.AreEqual(expected, result);
        }


        public class SomePOCO
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public DateTime? Date { get; set; }
        }

        [Test]
        public void GetRandom_GivenPOCOType_ShouldUseOnTheFlyGenericBuilderToGiveBackRandomItem()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var item = GetRandom<SomePOCO>();

            //---------------Test Result -----------------------
            Assert.IsNotNull(item);
            Assert.IsInstanceOf<SomePOCO>(item);
            // assert that *something* was set
            Assert.IsNotNull(item.Id);
            Assert.IsNotNull(item.Name);
            Assert.IsNotNull(item.Date);
        }

        [Test]
        public void GetRandomValue_GivenPOCOTypeArgument_ShouldUseOnTheFlyGenericBuilderToGiveBackRandomItem()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var item = GetRandomValue(typeof(SomePOCO)) as SomePOCO;

            //---------------Test Result -----------------------
            Assert.IsNotNull(item);
            Assert.IsInstanceOf<SomePOCO>(item);
            // assert that *something* was set
            Assert.IsNotNull(item.Id);
            Assert.IsNotNull(item.Name);
            Assert.IsNotNull(item.Date);
        }

        [TestCase(typeof(int))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(char))]
        [TestCase(typeof(long))]
        [TestCase(typeof(float))]
        [TestCase(typeof(double))]
        [TestCase(typeof(decimal))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(string))]
        [TestCase(typeof(bool))]
        public void GetRandomValue_GivenPrimitiveTypeArgument_ShouldUseRegularRVGMethods(Type type)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var item = GetRandomValue(type);

            //---------------Test Result -----------------------
            Assert.IsNotNull(item);
            Assert.IsInstanceOf(type, item);
        }

        [TestCase(typeof(int?))]
        [TestCase(typeof(byte?))]
        [TestCase(typeof(char?))]
        [TestCase(typeof(long?))]
        [TestCase(typeof(float?))]
        [TestCase(typeof(double?))]
        [TestCase(typeof(decimal?))]
        [TestCase(typeof(DateTime?))]
        [TestCase(typeof(bool?))]
        public void GetRandomValue_GivenNullablePrimitiveTypeArgument_ShouldUseRegularRVGMethods(Type type)
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var item = GetRandomValue(type);

            //---------------Test Result -----------------------
            Assert.IsNotNull(item);
            Assert.IsInstanceOf(type, item);
        }


        [Test]
        public void GetRandomValue_GivenPOCOType_ShouldHaveVariance()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var items = GetRandomCollection(GetRandom<SomePOCO>, NORMAL_RANDOM_TEST_CYCLES, NORMAL_RANDOM_TEST_CYCLES);

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant<SomePOCO, int>(items, "Id");
            VarianceAssert.IsVariant<SomePOCO, string>(items, "Name");
            VarianceAssert.IsVariant<SomePOCO, DateTime>(items, "Date");
        }

        public class SomePOCOWithBuilder : SomePOCO
        {
        }

        public class SomePOCOWithBuilderBuilder : GenericBuilder<SomePOCOWithBuilderBuilder, SomePOCOWithBuilder>
        {
            public override SomePOCOWithBuilderBuilder WithRandomProps()
            {
                return base.WithRandomProps()
                    .WithProp(o => o.Id = GetRandomInt(1000, 2000));
            }
        }

        [Test]
        public void GetRandomValue_GivenPOCOWithBuilderType_ShouldUseExistingBuilder()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var item = GetRandom<SomePOCOWithBuilder>();

            //---------------Test Result -----------------------
            Assert.IsNotNull(item);
            Assert.IsInstanceOf<SomePOCOWithBuilder>(item);
            // assert that *something* was set
            Assert.IsNotNull(item.Id);
            Assert.That(item.Id.Value, Is.GreaterThanOrEqualTo(1000));
            Assert.That(item.Id.Value, Is.LessThanOrEqualTo(2000));
            Assert.IsNotNull(item.Name);
            Assert.IsNotNull(item.Date);
        }

        [Test]
        public void Discovery_EncodingNonPrintableCharacters_ShouldNotThrow()
        {
            //---------------Set up test pack-------------------
            var bytes = GetRandomCollection(() => GetRandomInt(0, 255))
                .Select(i => (byte) i)
                .ToArray();
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = Encoding.UTF8.GetString(bytes);

            //---------------Test Result -----------------------
            Console.WriteLine(result);
        }


        [Test]
        public void GetRandomAlphaString_GivenMinLength_ShouldReturnValueOfAtLeastThatLength()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(
                () =>
                {
                    var minLength = GetRandomInt(10, 20);
                    var result = GetRandomAlphaNumericString(minLength);
                    Assert.That(result.Length, Is.GreaterThanOrEqualTo(minLength));
                });

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandomBytes_ShouldReturnBytesAcrossFullRange()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            for (var i = 0; i < 20; i++)
            {
                // look for full-range variance across an 8k block
                var result = GetRandomBytes(8192, 8192);
                if (result.Distinct().Count() == 256)
                    return;
            }

            //---------------Test Result -----------------------
            Assert.Fail("Couldn't find full range of bytes");
        }

        [Test]
        public void GetRandomString_ShouldProduceStringInRequiredLengthRange()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(
                () =>
                {
                    var minLength = GetRandomInt(10, 20);
                    var maxLength = GetRandomInt(21, 30);
                    var result = GetRandomString(minLength, maxLength);
                    Assert.That(result.Length, Is.GreaterThanOrEqualTo(minLength));
                    Assert.That(result.Length, Is.LessThanOrEqualTo(maxLength));
                });

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandomString_GivenMinAndMaxLengthsSwapped_ShouldProduceStringWithinRequiredLengthRange()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            RunCycles(
                () =>
                {
                    var minLength = GetRandomInt(10, 20);
                    var maxLength = GetRandomInt(21, 30);
                    var result = GetRandomString(maxLength, minLength);
                    Assert.That(result.Length, Is.GreaterThanOrEqualTo(minLength));
                    Assert.That(result.Length, Is.LessThanOrEqualTo(maxLength));
                });

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandomIPv4Address_ShouldReturnValidIPV4Addresses()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var allResults = new List<string>();
            RunCycles(
                () =>
                {
                    var result = GetRandomIPv4Address();
                    allResults.Add(result);
                    var parts = result.Split('.');
                    Assert.AreEqual(4, parts.Length);
                    var ints = parts.Select(int.Parse);
                    Assert.IsTrue(ints.All(i => i >= 0 && i < 265));
                });

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(allResults);
        }

        [Test]
        public void GetRandomHostName_ShouldReturnRandomHostName()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var allResults = new List<string>();
            RunCycles(
                () =>
                {
                    var result = GetRandomHostname();
                    Assert.IsNotNull(result);
                    var parts = result.Split('.');
                    Assert.IsTrue(parts.All(p => p.Length > 0));
                    allResults.Add(result);
                });

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(allResults);
        }

        public void GetRandomVersionString_GivenNoParameters_ShouldReturnVersionWithThreeIntegerParts()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var allResults = new List<string>();
            RunCycles(
                () =>
                {
                    var result = GetRandomVersionString();
                    var parts = result.Split('.');
                    Assert.AreEqual(3, parts.Length);
                    Assert.IsTrue(parts.All(p => p.IsInteger()));
                    allResults.Add(result);
                });

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(allResults);
        }

        [Test]
        public void GetRandomVersionString_GivenPartsCount_ShouldReturnVersionWithThatManyParts()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var allResults = new List<string>();
            RunCycles(
                () =>
                {
                    var partCount = GetRandomInt(2, 7);
                    var result = GetRandomVersionString(partCount);
                    var parts = result.Split('.');
                    Assert.AreEqual(partCount, parts.Length);
                    Assert.IsTrue(parts.All(p => p.IsInteger()));
                    allResults.Add(result);
                });

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(allResults);
        }

        [Test]
        public void GetRandomVersion_ShouldReturnRandomDotNetVersionInfo()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var allResults = new List<Version>();
            RunCycles(
                () =>
                {
                    var result = GetRandomVersion();
                    allResults.Add(result);
                });

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(allResults);
        }

        [Test]
        public void GetRandomFoldername_ShouldProduceVariantPath()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var allResults = new List<string>();
            RunCycles(
                () =>
                {
                    var thisResult = GetRandomWindowsPath();
                    var parts = thisResult.Split('\\');
                    Assert.That(parts.Length, Is.GreaterThan(1));
                    Assert.That(parts.Length, Is.LessThan(6));
                    Assert.That(thisResult.Length, Is.LessThan(248));
                    Assert.That(parts[0].Length == 2);
                    Assert.That(parts[0].EndsWith(":"));
                    StringAssert.Contains(parts[0].First().ToString(), "ABCDEGHIJKLMNOPQRSTUVWXYZ");
                    allResults.Add(thisResult);
                });

            //---------------Test Result -----------------------
            VarianceAssert.IsVariant(allResults);
        }

        [Test]
        public void GetRandom_OnInternalTypeFromAnotherAssembly()
        {
            //---------------Set up test pack-------------------

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.DoesNotThrow(
                () => GetRandom<InternalClass>()
            );

            //---------------Test Result -----------------------
        }

        [Test]
        public void GetRandom_OnInternalTypeNotShared()
        {
            //---------------Set up test pack-------------------
            if (Type.GetType("Mono.Runtime") != null)
            {
                Assert.Ignore("Mono allows access to internal types, so this test is skipped");
                return;
            }

            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            Assert.Throws<UnableToCreateDynamicBuilderException>(
                () => GetRandom<AnotherInternalClass>()
            );

            //---------------Test Result -----------------------
        }

        [Test]
        public void CreateRandomFolderIn_GivenExistingPath_ShouldCreateFolderInThere()
        {
            //---------------Set up test pack-------------------
            using (var folder = new AutoTempFolder())
            {
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = CreateRandomFolderIn(folder.Path);

                //---------------Test Result -----------------------
                Assert.IsNotNull(result);
                Assert.IsTrue(Directory.Exists(Path.Combine(folder.Path, result)));
            }
        }


        [Test]
        public void CreateRandomFoldersIn_GivenPath_ShouldCreateSomeRandomFoldersThereAndReturnTheRelativePaths()
        {
            //---------------Set up test pack-------------------
            using (var folder = new AutoTempFolder())
            {
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = CreateRandomFoldersIn(folder.Path);

                //---------------Test Result -----------------------
                Expect(result).Not.To.Be.Null();
                Expect(result).Not.To.Be.Empty();
                Expect(result).To.Have.Unique.Items();

                Expect(result).To.Be.FoldersUnder(folder.Path);
                Expect(result).To.Be.TheOnlyFoldersUnder(folder.Path);
            }
        }

        [Test]
        public void
            CreateRandomFoldersIn_GivenPathAndDepth_ShouldCreateSomeRandomFoldersThereAndReturnTheRelativePaths()
        {
            //---------------Set up test pack-------------------
            using (var folder = new AutoTempFolder())
            {
                //---------------Assert Precondition----------------
                var depth = GetRandomInt(2, 3);

                //---------------Execute Test ----------------------
                var result = CreateRandomFoldersIn(folder.Path, depth);

                //---------------Test Result -----------------------
                Expect(result).Not.To.Be.Null();
                Expect(result).Not.To.Be.Empty();
                Expect(result).To.Be.FoldersUnder(folder.Path);
                Expect(result).To.Have.Unique.Items();

                var depths = result
                    .Select(r => r.Split(Path.DirectorySeparatorChar).Length)
                    .ToArray();
                Expect(depths.All(d => d <= depth)).To.Be.True();
            }
        }

        [Test]
        public void CreateRandomFileIn_GivenPath_ShouldReturnNameOfFileCreatedThereWithRandomContents()
        {
            //---------------Set up test pack-------------------
            using (var folder = new AutoTempFolder())
            {
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = CreateRandomFileIn(folder.Path);

                //---------------Test Result -----------------------
                Assert.IsNotNull(result);
                Assert.IsTrue(File.Exists(Path.Combine(folder.Path, result)));
                CollectionAssert.IsNotEmpty(File.ReadAllBytes(Path.Combine(folder.Path, result)));
            }
        }

        [Test]
        public void CreateRandomTextFileIn_GivenPath_ShouldReturnNameOfFileCreatedThereWithRandomContents()
        {
            //---------------Set up test pack-------------------
            using (var folder = new AutoTempFolder())
            {
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = CreateRandomTextFileIn(folder.Path);

                //---------------Test Result -----------------------
                Assert.IsNotNull(result);
                Assert.IsTrue(File.Exists(Path.Combine(folder.Path, result)));
                var lines = File.ReadAllLines(Path.Combine(folder.Path, result));
                CollectionAssert.IsNotEmpty(lines);
                Assert.IsTrue(
                    lines.All(
                        l =>
                        {
                            return l.All(c => !Char.IsControl(c));
                        }));
            }
        }

        [Test]
        public void CreateRandomFileTreeIn_GivenPath_ShouldCreateFilesAndFoldersAndReturnTheirRelativePaths()
        {
            //---------------Set up test pack-------------------
            using (var folder = new AutoTempFolder())
            {
                //---------------Assert Precondition----------------

                //---------------Execute Test ----------------------
                var result = CreateRandomFileTreeIn(folder.Path);

                //---------------Test Result -----------------------
                Assert.IsNotNull(result);
                CollectionAssert.IsNotEmpty(result);
                Assert.IsTrue(result.Any(r => PathExists(Path.Combine(folder.Path, r))));
                Assert.IsTrue(result.Any(r => File.Exists(Path.Combine(folder.Path, r))), "No files found");
                Assert.IsTrue(result.Any(r => Directory.Exists(Path.Combine(folder.Path, r))), "No folders found");
            }
        }

        [Test]
        public void RangeCheckTimeOnRandomDate_WhenGivenDateWithTimeExceedingMaxTime_ShouldReturnDateWithTimeAtMaxTime()
        {
            RunCycles(
                () =>
                {
                    //---------------Set up test pack-------------------
                    var input = new DateTime(2011, 1, 1, 12, 30, 0);
                    var maxTime = new DateTime(2011, 1, 1, 9, 30, 0);

                    //---------------Assert Precondition----------------

                    //---------------Execute Test ----------------------
                    var result = RangeCheckTimeOnRandomDate(null, maxTime, input);

                    //---------------Test Result -----------------------
                    Assert.That(result.TimeOfDay, Is.LessThanOrEqualTo(maxTime.TimeOfDay));
                });
        }

        public interface IInterfaceToGetRandomOf
        {
            string Name { get; }
        }

        public class ImplementingType : IInterfaceToGetRandomOf
        {
            public string Name { get; set; }
        }

        [Test]
        public void
            GetRandomOfType_GivenInterfaceType_WhenImplementationWithDefaultConstructorCanBefound_ShouldReturnInstance_AndPreferAccessModifiersFromInstance()
        {
            //---------------Set up test pack-------------------
            //---------------Assert Precondition----------------

            //---------------Execute Test ----------------------
            var result = GetRandom<IInterfaceToGetRandomOf>();

            //---------------Test Result -----------------------
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Name);
        }

        public interface IHasAnId
        {
            int Id { get; set; }
        }

        [Test]
        public void GetRandom_GivenAValidatorFunction_ShouldReturnADifferentValue()
        {
            RunCycles(
                () =>
                {
                    //--------------- Arrange -------------------
                    var first = GetRandom<IHasAnId>();

                    //--------------- Assume ----------------

                    //--------------- Act ----------------------
                    var other = GetRandom<IHasAnId>(test =>
                    {
                        return test.Id != first.Id;
                    });

                    //--------------- Assert -----------------------
                    Expect(other).Not.To.Be.Null();
                    Expect(other).Not.To.Equal(first);
                    Expect(other.Id).Not.To.Equal(first.Id);
                });
        }

        [Test]
        public void GetRandom_GivenAValidatorAndGenerator_ShouldUseTheGeneratorToReturnADifferentValue()
        {
            //--------------- Arrange -------------------
            var first = GetRandom<IHasAnId>();
            var expected = GetRandom<IHasAnId>(o => o.Id != first.Id);

            //--------------- Assume ----------------
            // rather fail early if we're about to enter an infinite loop
            Expect(first.Id).Not.To.Equal(expected.Id);

            //--------------- Act ----------------------
            var result = GetRandom(
                o => o.Id != first.Id,
                () => expected
            );

            //--------------- Assert -----------------------
            Expect(result).To.Equal(expected);
        }

        public class HasConstructorWithParameter
        {
            public string Parameter { get; }

            public HasConstructorWithParameter(string parameter)
            {
                Parameter = parameter;
            }
        }

        [Test]
        public void GetRandomOfType_WhenTypeHasSimpleParameteredConstructor_ShouldAttemptToConstruct()
        {
            // Arrange
            // Pre-Assert
            // Act
            var result = GetRandom<HasConstructorWithParameter>();
            // Assert
            Expect(result.Parameter).Not.To.Be.Null();
        }

        [Test]
        public void GetRandomOfTypeKeyValuePair_ShouldReturnKeyValuePairWithData()
        {
            // Arrange
            // Pre-Assert
            // Act
            var result = GetRandom<KeyValuePair<string, string>>();
            var attempts = 0;
            while (result.Key == "" || result.Value == "")
            {
                if (++attempts > 10)
                    Assert.Fail("Unable to get non-empty key or value");
                result = GetRandom<KeyValuePair<string, string>>();
            }

            // Assert
            Expect(result).Not.To.Be.Null();
            Expect(result.Key).Not.To.Be.Null();
            Expect(result.Value).Not.To.Be.Null();
        }

        public class HasTwoConstructors
        {
            public bool ParameterlessConstructorUsed { get; }
            public string Parameter { get; }

            public HasTwoConstructors()
            {
                ParameterlessConstructorUsed = true;
            }

            public HasTwoConstructors(string parameter)
            {
                ParameterlessConstructorUsed = false;
                Parameter = parameter;
            }
        }

        [Test]
        public void GetRandomOfT_ShouldPreferTheParameterlessConstructor()
        {
            // Arrange
            // Pre-Assert
            // Act
            var sut = GetRandom<HasTwoConstructors>();
            // Assert
            Expect(sut.ParameterlessConstructorUsed).To.Be.True();
        }

        [Test]
        public void WhenPropModsInvokeWithProp_ShouldNotThrowCollectionModifiedException()
        {
            // Arrange

            // Pre-assert

            // Act
            Expect(
                    () =>
                    {
                        var parent = GetRandom<Parent>();
                        Expect(parent.Children).Not.To.Be.Empty();
                    })
                .Not.To.Throw();

            // Assert
        }

        public class PocoWithTimeSpan
        {
            public TimeSpan Moo { get; set; }
        }

        [Test]
        public void GetRandomOfT_WhenTypeHasTimeSpanProperty_ShouldNotExplode()
        {
            // Arrange
            // Pre-assert
            // Act
            Expect(GetRandom<PocoWithTimeSpan>)
                .Not.To.Throw();
            // Assert
        }

        [Test]
        public void WhenPropModsInvokeWithProp_ShouldNotStoreLastBuildTimePropMods()
        {
            // Arrange
            var builder = ParentBuilder.Create().WithRandomProps();

            // Pre-assert

            // Act
            builder.Build();
            Expect(builder.WithChildrenCallCount).To.Equal(1);
            builder.Build();

            // Assert
            Expect(builder.WithChildrenCallCount).To.Equal(2);
        }
        
        public class Parent
        {
            public string Name { get; set; }
            public ChildNode[] Children { get; set; }
        }

        public class ChildNode
        {
            public string Name { get; set; }
        }

        public class ParentBuilder : GenericBuilder<ParentBuilder, Parent>
        {
            public int WithChildrenCallCount { get; private set; }

            public override ParentBuilder WithRandomProps()
            {
                return base.WithRandomProps()
                    .WithRandomChildren();
            }

            public ParentBuilder WithRandomChildren()
            {
                return WithProp(o => WithChildren(GetRandomCollection<ChildNode>(2, 4).ToArray()));
            }

            public ParentBuilder WithChildren(params ChildNode[] nodes)
            {
                WithChildrenCallCount++;
                return WithProp(o => o.Children = o.Children.EmptyIfNull().And(nodes));
            }
        }

        private bool PathExists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }


        private string BuildErrorMessageFor(
            IEnumerable<Tuple<string, int, int>> tooShort,
            IEnumerable<Tuple<string, int, int>> tooLong,
            IEnumerable<Tuple<string, int, int>> invalidCharacters)
        {
            var message = new List<string>();
            if (tooShort.Any())
            {
                message.Add(
                    string.Join(
                        "\n",
                        "Some results were too short:",
                        string.Join("\n\t", tooShort.Take(5).Select(i => $"{i.Item1}  (<{i.Item2})"))));
            }

            if (tooLong.Any())
            {
                message.Add(
                    string.Join(
                        "\n",
                        "Some results were too long:",
                        string.Join("\n\t", tooLong.Take(5).Select(i => $"{i.Item1}  (>{i.Item3})"))));
            }

            if (invalidCharacters.Any())
            {
                message.Add(
                    string.Join(
                        "\n",
                        "Some results contained invalid characters:",
                        string.Join("\n\t", invalidCharacters.Take(5).Select(i => i.Item1))));
            }

            return message.JoinWith("\n");
        }


        private void RunCycles(Action toRun, int? cycles = 0)
        {
            cycles = cycles ?? NORMAL_RANDOM_TEST_CYCLES;
            for (var i = 0; i < NORMAL_RANDOM_TEST_CYCLES; i++)
                toRun();
        }

        private string GetErrorHelpFor(
            IEnumerable<DateTime> outOfRangeLeft,
            IEnumerable<DateTime> outOfRangeRight,
            DateTime minTime,
            DateTime maxTime)
        {
            var message = "";
            if (outOfRangeLeft.Any())
            {
                message = string.Join(
                    "\n",
                    "One or more results had a time that was too early:",
                    "minTime: " + minTime.ToString("yyyy/MM/dd HH:mm:ss.ttt"),
                    "bad values: " + string.Join(",", outOfRangeLeft.Take(5)));
            }

            if (outOfRangeRight.Any())
            {
                message += string.Join(
                    "\n",
                    "One or more results had a time that was too late:",
                    "maxTime: " + maxTime.ToString("yyyy/MM/dd HH:mm:ss.ttt"),
                    "bad values: " + string.Join(",", outOfRangeLeft.Take(5)));
            }

            return message;
        }

        private class DateTimeRangeContainer
        {
            public DateTime From { get; }
            public DateTime To { get; }

            public DateTimeRangeContainer(int minYear, int minMonth, int minDay, int maxYear, int maxMonth, int maxDay)
            {
                From = new DateTime(minYear, minMonth, minDay, 0, 0, 0);
                To = new DateTime(maxYear, maxMonth, maxDay, 0, 0, 0);
                if (From > To)
                {
                    var swap = From;
                    From = To;
                    To = swap;
                }
            }

            public bool InRange(DateTime value)
            {
                return value >= From && value <= To;
            }
        }
        
        [TestFixture]
        public class GettingRandomHttpUrls
        {
            [Repeat(NORMAL_RANDOM_TEST_CYCLES)]
            [Test]
            public void ShouldReturnAValidUrl()
            {
                // Arrange
                // Pre-assert
                // Act
                var url = GetRandomHttpUrl();
                // Assert
                Expect(url).Not.To.Be.Null.Or.Empty();
                Expect(() => new Uri(url))
                    .Not.To.Throw();
            }

            [Repeat(NORMAL_RANDOM_TEST_CYCLES)]
            [Test]
            public void ShouldReturnUrlInLowerCaseOnly()
            {
                // Arrange
                // Pre-assert
                // Act
                var url = GetRandomHttpUrl();
                // Assert
                Expect(url).To.Equal(url.ToLowerInvariant());
            }
        }

        [TestFixture]
        public class GettingRandomHttpUrlsWithParameters
        {
            [Repeat(NORMAL_RANDOM_TEST_CYCLES)]
            [Test]
            public void ShouldReturnAValidUrl()
            {
                // Arrange
                // Pre-assert
                // Act
                var url = GetRandomHttpUrlWithParameters();
                // Assert
                Expect(() => new Uri(url))
                    .Not.To.Throw();
            }

            [Repeat(NORMAL_RANDOM_TEST_CYCLES)]
            [Test]
            public void ShouldReturnUrlInLowerCaseOnly()
            {
                // Arrange
                // Pre-assert
                // Act
                var url = GetRandomHttpUrlWithParameters();
                // Assert
                var uri = new Uri(url);
                var schemeHostPath = uri.ToString().Replace(uri.Query, "");
                Expect(schemeHostPath).To.Equal(schemeHostPath.ToLowerInvariant());
            }

            [Repeat(NORMAL_RANDOM_TEST_CYCLES)]
            [Test]
            public void ShouldHaveAtLeastOneParameter()
            {
                // Arrange
                // Pre-assert
                // Act
                var url = GetRandomHttpUrlWithParameters();
                // Assert
                var uri = new Uri(url);
                Expect(uri.Query).Not.To.Be.Null.Or.Empty();
                var parameters = uri.Query.Substring(1)
                    .Split(new[] {"&"}, StringSplitOptions.RemoveEmptyEntries);
                Expect(parameters).Not.To.Be.Empty();
            }
            
        }
    }

    internal static class Matchers
    {
        internal static void FoldersUnder(
            this ICollectionBe<string> be,
            string basePath
        )
        {
            be.Compose(
                actual =>
                {
                    actual.ForEach(
                        sub =>
                            Expect(Path.Combine(basePath, sub)).To.Be.A.Folder()
                    );
                });
        }

        internal static void TheOnlyFoldersUnder(
            this ICollectionBe<string> be,
            string folder
        )
        {
            be.Compose(
                actual =>
                {
                    var existing = Directory.EnumerateDirectories(
                            folder,
                            "*",
                            SearchOption.AllDirectories
                        )
                        .Select(
                            p => p.Substring(folder.Length + 1)
                        );
                    Expect(existing).To.Be.Equivalent.To(actual);
                });
        }

        internal static void Folder(
            this IA<string> a
        )
        {
            a.Compose(
                path =>
                    Expect(Directory.Exists(path)).To.Be.True()
            );
        }
    }
}