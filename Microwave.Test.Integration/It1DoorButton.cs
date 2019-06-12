using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class It1DoorButton
    {
        private ICookController _cooker;
        private ILight _light;
        private IDisplay _display;

        private IButton _startCancelButton;
        private IButton _powerButton;
        private IButton _timeButton;
        private IDoor _door;


        private UserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _cooker = Substitute.For<ICookController>();
            _light = Substitute.For<ILight>();
            _display = Substitute.For<IDisplay>();

            _startCancelButton = new Button();
            _powerButton = new Button();
            _timeButton = new Button();
            _door = new Door();
            _userInterface = new UserInterface(
                _powerButton, _timeButton, _startCancelButton,
                _door, _display, _light, _cooker);
        }

        [Test]
        public void OnDoorOpened_Ready_LightTurnsOn()
        {
            _door.Open();
            _light.Received(1).TurnOn();
        }


        [Test]
        public void OnDoorOpened_SetPower_LightTurnsOn()
        {
            _powerButton.Press();
            _door.Open();
            _light.Received(1).TurnOn();
        }

        [Test]
        public void OnDoorOpened_SetTime_LightTurnsOn()
        {
            _powerButton.Press();
            _timeButton.Press();
            _door.Open();
            _light.Received(1).TurnOn();
        }

        [Test]
        public void OnDoorOpened_Cooking_CookingStops()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _door.Open();
            _cooker.Received().Stop();
        }

        [Test]
        public void OnDoorOpened_DoorIsOpen_DoesNotThrow()
        {
            _door.Open();
            Assert.DoesNotThrow(() => _door.Open());
        }


        [Test]
        public void OnDoorClosed_DoorOpen_LightTurnsOff()
        {
            _door.Open();
            _door.Close();
            _light.Received(1).TurnOff();
        }

        [Test]
        public void OnDoorClosed_WasClosed_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _door.Close());
        }

        [Test]
        public void OnPowerPressed_Ready_DisplayShowsPower()
        {
            _powerButton.Press();
            _display.Received().ShowPower(Arg.Any<int>());
        }

        [Test]
        public void OnPowerPressed_SetPower_PowerIsCorrect()
        {
            _powerButton.Press();
            _powerButton.Press();

            _display.Received().ShowPower(100);
        }

        [Test]
        public void OnTimePressed_SetPower_TimeIsShowed()
        {
            _powerButton.Press();
            _timeButton.Press();
            _display.Received().ShowTime(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void OnTimePressed_SetTime_TimeIsCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _timeButton.Press();
            _display.Received().ShowTime(2, 0);
        }

        [Test]
        public void OnStartCancelPressed_SetPower_DisplayClears()
        {
            _powerButton.Press();
            _startCancelButton.Press();
            _display.Received(1).Clear();
        }

        [Test]
        public void OnStartCancelPressed_SetTime_CookControllerStarts()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _cooker.StartCooking(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void OnStartCancelPressed_Cooking_CookControllerStops()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();
            _cooker.Received().Stop();
        }
    }
}
