using NUnit.Framework;
using System;
using System.Threading.Tasks;
using LandingAssignment.Demo;
namespace Landing.Test
{
    [TestFixture]
    public class Tests
    {
     

        [Test]
        [TestCase(-1,1,2,3,4,5)]
        [TestCase(1, -1, 2, 3, 4, 5)]
        [TestCase(1, 1, -1, 3, 4, 5)]
        [TestCase(1, 1, 2, -1, 4, 5)]
        [TestCase(1, 1, 2, 3, -1, 5)]
        [TestCase(1, 1, 2, 3, 4, -1)]
       
        public void Given_Incorrect_Dimension_When_Tries_To_Create_Landing_Instance_Then_Throws_Argument_Exception(int widthOfArea,int heightOfArea,int widthOfLandingArea,int heightOfLandingArea,int xOfLandingPosition,int yOfLandingPosition)
        {
            var thrownException= Assert.Throws<ArgumentException>(()=>
            new LandingAssignment.Demo.Landing(widthOfArea, heightOfArea, heightOfLandingArea, widthOfLandingArea, new Position() { X = xOfLandingPosition, Y = yOfLandingPosition }));

            Assert.That(thrownException.Message, Is.EqualTo("Dimensions and position should be positive number!"));
        }

        [Test]
        [TestCase(5, 5, 6, 5, 4, 5)]
        [TestCase(5, 5, 5, 6, 4, 5)]

        public void Given_Wider_Landing_Area_When_Tries_To_Create_Landing_Instance_Then_Throws_Argument_Exception(int widthOfArea, int heightOfArea, int widthOfLandingArea, int heightOfLandingArea, int xOfLandingPosition, int yOfLandingPosition)
        {
            var thrownException = Assert.Throws<ArgumentException>(() =>
            new LandingAssignment.Demo.Landing(widthOfArea, heightOfArea, heightOfLandingArea, widthOfLandingArea, new Position() { X = xOfLandingPosition, Y = yOfLandingPosition }));
            Assert.That(thrownException.Message, Is.EqualTo("Landing platform should be smaller than the area!"));
        }

        [Test]
        [TestCase(100, 100, 6, 5, 99, 5)]
        [TestCase(100, 100, 6, 5, 4, 99)]

        public void Given_Overflowing_Landing_Area_When_Tries_To_Create_Landing_Instance_Then_Throws_Argument_Exception(int widthOfArea, int heightOfArea, int widthOfLandingArea, int heightOfLandingArea, int xOfLandingPosition, int yOfLandingPosition)
        {
            var thrownException = Assert.Throws<ArgumentException>(() =>
            new LandingAssignment.Demo.Landing(widthOfArea, heightOfArea, heightOfLandingArea, widthOfLandingArea, new Position() { X = xOfLandingPosition, Y = yOfLandingPosition }));
            Assert.That(thrownException.Message, Is.EqualTo("Landing platform should be located inside area!"));
        }

        [Test]
        [TestCase(5, 5)]
        [TestCase(10, 10)]
        [TestCase(7, 7)]
        [TestCase(9, 10)]
        [TestCase(5, 6)]

        public void Given_Correct_Position_When_Requests_Landing_Then_Returns_Ok(int xOfRequetedPosition,int yOfRequestedPosition)
        {

            int planeId = 1;
            var landingInstance = new LandingAssignment.Demo.Landing(100, 100, 10, 10, new Position() { X = 0, Y = 0 });
            var result = landingInstance.RequestLanding(planeId, new Position() { X = xOfRequetedPosition, Y = yOfRequestedPosition });

            Assert.That(result, Is.EqualTo(LandingRequestResult.Ok.GetDisplayName()));
        }
        [Test]
        [TestCase(5, 5)]
        [TestCase(10, 10)]
        [TestCase(7, 7)]
        [TestCase(4, 5)]
        [TestCase(5, 4)]

        public void Given_Out_Of_Landing_Area_Position_When_Requests_Landing_Then_Returns_Out_Of_Platform(int xOfRequetedPosition, int yOfRequestedPosition)
        {

            int planeId = 1;
            var landingInstance = new LandingAssignment.Demo.Landing(100, 100, 4, 4, new Position() { X = 0, Y = 0 });
            var result = landingInstance.RequestLanding(planeId, new Position() { X = xOfRequetedPosition, Y = yOfRequestedPosition });

            Assert.That(result, Is.EqualTo(LandingRequestResult.OutOfPlatform.GetDisplayName()));
        }
        [Test]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(3, 5)]
        [TestCase(4, 3)]
        [TestCase(4, 5)]
        [TestCase(5, 3)]
        [TestCase(5, 4)]
        [TestCase(5, 5)]
        public void Given_Near_Position_To_Lastly_Requested_Position_When_Requests_Landing_Then_Returns_Clash(int xOfRequetedPosition, int yOfRequestedPosition)
        {

            int firstPlaneId = 1;
            int secondPlaneId = 2;
            var landingInstance = new LandingAssignment.Demo.Landing(100, 100, 10, 10, new Position() { X = 0, Y = 0 });
            landingInstance.RequestLanding(firstPlaneId, new Position() { X = 4, Y = 4});
            var result=landingInstance.RequestLanding(secondPlaneId, new Position() { X = xOfRequetedPosition, Y = yOfRequestedPosition });

            Assert.That(result, Is.EqualTo(LandingRequestResult.Clash.GetDisplayName()));
        }
        [Test]
        [TestCase(3, 3)]
        [TestCase(3, 4)]
        [TestCase(3, 5)]
        [TestCase(4, 3)]
        [TestCase(4, 5)]
        [TestCase(5, 3)]
        [TestCase(5, 4)]
        [TestCase(5, 5)]
        public async Task Given_Long_Running_Near_Position_Request_To_Simultaneusly_Fast_Running_Position_Request_When_Requests_Landing_Then_Returns_Clash(int xOfRequetedPosition, int yOfRequestedPosition)
        {

            int firstPlaneId = 1;
            int secondPlaneId = 2;
            var landingInstance = new LandingAssignment.Demo.Landing(100, 100, 10, 10, new Position() { X = 0, Y = 0 });
            var longRunningTaskResult= RunRequestLandingAfterDelay(landingInstance, secondPlaneId, new Position() { X = xOfRequetedPosition, Y = yOfRequestedPosition });
            var resultFastRunning=landingInstance.RequestLanding(firstPlaneId, new Position() { X = 4, Y = 4 });
            longRunningTaskResult.Wait();


            Assert.That(longRunningTaskResult.Result, Is.EqualTo(LandingRequestResult.Clash.GetDisplayName()));
        }

        private async Task<string> RunRequestLandingAfterDelay(LandingAssignment.Demo.Landing instance,int planeId,Position requestedPosition)
        {
          
                await Task.Delay(1000);
               return instance.RequestLanding(planeId, requestedPosition);
        }

    }
}