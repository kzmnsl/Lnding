using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace LandingAssignment.Demo
{
    public class Position{
        public int X { get; set; }
        public int Y { get; set; }
    }
    public enum LandingRequestResult{
        [Description("ok for landing")]
        Ok,
        [Description("clash")]
        Clash,
        [Description("out of platform ")]
        OutOfPlatform
    }

    public class Landing
    {
        private readonly object lastLandingLock = new object();
        private Dictionary<int,Position> lastLandingRequests= new Dictionary<int, Position>();
        private readonly int _widthOfArea;
        private readonly int _heightOfArea;
        private readonly int _heightOfLandingArea;
        private readonly int _widthOfLandingArea;

        private readonly Position _startingPositionOfLandingArea;

        public Landing(int widthOfArea,int heightOfArea,int heightOfLandingArea,int widthOfLandingArea,Position startingPositionOfLandingArea){
            if (widthOfArea <= 0 || heightOfArea <= 0 || heightOfLandingArea <= 0 || widthOfArea <= 0 || widthOfLandingArea <= 0 || startingPositionOfLandingArea.X < 0 || startingPositionOfLandingArea.Y < 0)
            {
                throw new ArgumentException("Dimensions and position should be positive number!");
            }
            if (widthOfArea<widthOfLandingArea || heightOfArea< heightOfLandingArea){
                throw new ArgumentException("Landing platform should be smaller than the area!");
            }
            if (startingPositionOfLandingArea.X+widthOfLandingArea > widthOfArea || startingPositionOfLandingArea.Y + heightOfLandingArea > heightOfArea)
            {
                throw new ArgumentException("Landing platform should be located inside area!");
            }

            _widthOfArea =widthOfArea;
            _heightOfArea=heightOfArea;
            _widthOfLandingArea=widthOfLandingArea;
            _heightOfLandingArea= heightOfLandingArea;
            _startingPositionOfLandingArea= startingPositionOfLandingArea;
        }

        public string RequestLanding(int planeId,Position requestedPosition){
            
            if (requestedPosition.X<_startingPositionOfLandingArea.X ||
                    requestedPosition.X>_startingPositionOfLandingArea.X+_widthOfLandingArea||
                    requestedPosition.Y>_startingPositionOfLandingArea.Y+_heightOfLandingArea||
                    requestedPosition.Y < _startingPositionOfLandingArea.Y)
            {
                UpdateLastPositionRequestDictionary(planeId, requestedPosition);
                return LandingRequestResult.OutOfPlatform.GetDisplayName();
            }
                      

            lock(lastLandingLock){
                var lastRequestedPositonByOtherPlane=lastLandingRequests.Where( lastLandingRequest => lastLandingRequest.Key!=planeId && 
                                                    (Math.Abs(lastLandingRequest.Value.X-requestedPosition.X)==1 || lastLandingRequest.Value.X==requestedPosition.X) && 
                                                    (Math.Abs(lastLandingRequest.Value.Y-requestedPosition.Y)==1 || lastLandingRequest.Value.Y==requestedPosition.Y) ).FirstOrDefault();
                if(lastRequestedPositonByOtherPlane.Value!=null){
                    UpdateLastPositionRequestDictionary(planeId, requestedPosition);
                    return LandingRequestResult.Clash.GetDisplayName();
                }
                UpdateLastPositionRequestDictionary(planeId, requestedPosition);
                return LandingRequestResult.Ok.GetDisplayName();
            }
        }
        private void UpdateLastPositionRequestDictionary(int planeId,Position requestedPosition)
        {
            if (lastLandingRequests.ContainsKey(planeId))
                lastLandingRequests[planeId] = requestedPosition;
            else
                lastLandingRequests.Add(planeId, requestedPosition);
        }
    }
}
