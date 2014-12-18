﻿namespace WorkoutWotch.UnitTests.Models.Parsers
{
    using System;
    using Kent.Boogaart.PCLMock;
    using NUnit.Framework;
    using Sprache;
    using WorkoutWotch.Models.Actions;
    using WorkoutWotch.Models.Parsers;
    using WorkoutWotch.UnitTests.Services.Container.Mocks;

    [TestFixture]
    public class ActionParserFixture
    {
        [Test]
        public void ctor_throws_if_indent_level_is_less_than_zero()
        {
            Assert.Throws<ArgumentException>(() => ActionParser.GetParser(-1, new ContainerServiceMock()));
        }

        [Test]
        public void ctor_throws_if_container_service_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => ActionParser.GetParser(0, null));
        }

        [TestCase("Break for 10s", typeof(BreakAction))]
        [TestCase("Metronome at 1s, 2s, 3s", typeof(MetronomeAction))]
        [TestCase("Prepare for 10s", typeof(PrepareAction))]
        [TestCase("Say 'foo'", typeof(SayAction))]
        [TestCase("Wait for 10s", typeof(WaitAction))]
        [TestCase("Sequence:\n  * Say 'foo'\n  * Say 'bar'", typeof(SequenceAction))]
        [TestCase("Parallel:\n  * Say 'foo'\n  * Say 'bar'", typeof(ParallelAction))]
        public void can_parse_valid_input(string input, Type expectedType)
        {
            var result = ActionParser.GetParser(0, new ContainerServiceMock(MockBehavior.Loose))(new Input(input));
            Assert.True(result.WasSuccessful);
            Assert.True(result.Remainder.AtEnd);
            Assert.NotNull(result.Value);
            Assert.AreSame(expectedType, result.Value.GetType());
        }

        [TestCase("")]
        [TestCase("foo")]
        [TestCase("* Say 'foo'")]
        public void cannot_parse_invalid_input(string input)
        {
            var result = ActionParser.GetParser(0, new ContainerServiceMock(MockBehavior.Loose))(new Input(input));
            Assert.False(result.WasSuccessful);
        }
    }
}