using System;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class It2UserInterface
    {
        private IOutput output;
        private ITimer timer;
        private IPowerTube powerTube;

        private IButton startCancelButton;
        private IButton powerButton;
        private IButton timeButton;
        private IDoor door;

        private Light light;
        private Display display;
        private CookController cooker;
        private UserInterface userInterface;

        [SetUp]
        public void Setup()
        {
            timer = Substitute.For<ITimer>();
            powerTube = Substitute.For<IPowerTube>();
            output = Substitute.For<IOutput>();

            startCancelButton = new Button();
            powerButton = new Button();
            timeButton = new Button();
            door = new Door();

            light = new Light(output);
            display = new Display(output);
            cooker = new CookController(timer, display, powerTube);
            userInterface = new UserInterface(
                powerButton, timeButton, startCancelButton,
                door, display, light, cooker);

            cooker.UI = userInterface;
        }

        [Test]
        public void StartButtonPressed_SetTime_PowerTubeStartsWithCorrectPower()
        {
            //Set power to 100
            powerButton.Press();
            powerButton.Press();

            timeButton.Press();
            startCancelButton.Press();
            powerTube.Received().TurnOn(100);
        }

        [Test]
        public void StartButtonPressed_SetTime_TimerStartWithCorrectTime()
        {
            powerButton.Press();

            //Set time to 2
            timeButton.Press();
            timeButton.Press();
            startCancelButton.Press();
            timer.Received().Start(120);
        }

        [Test]
        public void DoorOpen_Cooking_PowerTubeTurnsOff()
        {
            powerButton.Press();
            timeButton.Press();
            startCancelButton.Press();
            door.Open();
            powerTube.Received().TurnOff();
        }

        [Test]
        public void StartCancelButtonClicked_Cooking_PowerTubeTurnsOff()
        {
            powerButton.Press();
            timeButton.Press();
            startCancelButton.Press();
            startCancelButton.Press();
            powerTube.Received().TurnOff();
        }

        [Test]
        public void DoorOpen_Cooking_TimeIsStopped()
        {
            powerButton.Press();

            timeButton.Press();
            startCancelButton.Press();
            startCancelButton.Press();
            timer.Received().Stop();
        }

        [Test]
        public void CookingIsDone_Cooking_PowerTubeTurnsOff()
        {
            powerButton.Press();

            timeButton.Press();
            startCancelButton.Press();

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);

            powerTube.Received().TurnOff();
        }


        [Test]
        public void CookingIsDone_AnythingButCooking_PowerTubeNotCalled()
        {
            timer.Expired += Raise.EventWith(this, EventArgs.Empty);
            output.DidNotReceive().OutputLine(Arg.Any<string>());
        }

        [Test]
        public void CookingIsDone_Cooking_LightOutputIsCorrect()
        {
            powerButton.Press();
            timeButton.Press();
            startCancelButton.Press();

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);
            output.Received().OutputLine(Arg.Is<string>(str => str.ToLower().Contains("off")));
        }


        [Test]
        public void CookingIsDone_Cooking_DisplayOutputIsCorrect()
        {
            powerButton.Press();
            timeButton.Press();
            startCancelButton.Press();

            timer.Expired += Raise.EventWith(this, EventArgs.Empty);
            output.Received().OutputLine(Arg.Is<string>(str => str.ToLower().Contains("cleared")));

        }

        [Test]
        public void TimeButtonPressed5Times_SetTime_TimeIsDisplayedCorrect()
        {
            powerButton.Press();

            //Set time to 5
            for (int i = 0; i < 5; i++)
            {
                timeButton.Press();
            }

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("05:00")));
        }

        [Test]
        public void PowerButtonPressed5Times_SetPower_PowerIsDisplayedCorrectly()
        {
            for (int i = 0; i < 5; i++)
            {
                powerButton.Press();
            }

            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("250 W")));
        }

        [Test]
        public void DoorOpens_Cooking_DisplayClears()
        {
            powerButton.Press();

            //Set time to 2
            timeButton.Press();
            startCancelButton.Press();
            door.Open();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("cleared")));
        }

        [Test]
        public void StartCancelButtonPressed_Cooking_DisplayClears()
        {
            powerButton.Press();
            timeButton.Press();
            startCancelButton.Press();
            startCancelButton.Press();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("cleared")));
        }

        [Test]
        public void DoorOpens_Ready_LightTurnsOn()
        {
            door.Open();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("on")));
        }

        [Test]
        public void DoorOpens_SetTime_LightTurnOn()
        {
            powerButton.Press();
            timeButton.Press();
            door.Open();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("on")));
        }
        [Test]
        public void DoorOpens_SetTime_DisplayCleared()
        {
            powerButton.Press();
            timeButton.Press();
            door.Open();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("cleared")));
        }

        [Test]
        public void DoorOpens_SetPower_LightTurnOn()
        {
            powerButton.Press();
            door.Open();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("on")));
        }

        [Test]
        public void DoorOpens_SetPower_DisplayCleared()
        {
            powerButton.Press();
            door.Open();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("cleared")));

        }

        [Test]
        public void DoorCloses_DoorIsOpen_LightTurnsOff()
        {
            door.Open();
            door.Close();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("off")));
        }

        [Test]
        public void StartCancelButtonPressed_SetTime_LightTurnsOn()
        {
            powerButton.Press();
            timeButton.Press();
            startCancelButton.Press();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("on")));
        }

        [Test]
        public void StartCancelButton_Cooking_LightTurnsOff()
        {
            powerButton.Press();
            timeButton.Press();
            startCancelButton.Press();
            startCancelButton.Press();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.ToLower().Contains("off")));
        }
    }
}