using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace Day10
{
    public class MonitoringStation
    {
        public static ((int X, int Y) bestAsteroid, int maxNumberOfVisible) CalculateBestStation(string map)
        {
            var asteroids = ProcessMap(map.Trim());

            var maxNumberOfVisible = 0;
            (int X, int Y)? bestAsteroid = null;

            foreach (var stationBaseCandidate in asteroids)
            {
                var distancesAndDirections =
                    from otherAsteroid in asteroids
                    where otherAsteroid != stationBaseCandidate
                    let deltaX = stationBaseCandidate.X - otherAsteroid.X
                    let deltaY = stationBaseCandidate.Y - otherAsteroid.Y
                    let distanceSquared = deltaX ^2 + deltaY ^2
                    let direction = Math.Atan2(deltaX, deltaY)
                    select (distanceSquared, direction);

                var asteroidsGroupedByDirection = distancesAndDirections.GroupBy(d => d.direction).ToList();
                var groups = asteroidsGroupedByDirection.Count();

                if (groups > maxNumberOfVisible)
                {
                    maxNumberOfVisible = groups;
                    bestAsteroid = stationBaseCandidate;
                }
            }

            return (bestAsteroid.Value, maxNumberOfVisible);
        }

        // Issues: which way is up? (map vs atan2 vs directionindex starting position.
        // laserCoordinates.X - otherAsteroid.X or otherAsteroid.X - laserCoordinates?

        public static (int X, int Y) DoGiantLaserStuff(string map, (int X, int Y) laserCoordinates)
        {
            var asteroids = ProcessMap(map.Trim());

            var distancesAndDirections =
                (from otherAsteroid in asteroids
                where otherAsteroid != laserCoordinates
                let deltaX = laserCoordinates.X - otherAsteroid.X
                let deltaY = laserCoordinates.Y - otherAsteroid.Y
                let distanceSquared = deltaX ^ 2 + deltaY ^ 2
                let direction = Math.Atan2(deltaX, deltaY)
                select (distanceSquared, direction, otherAsteroid)).ToList();

            var sortedDirections = distancesAndDirections.Select(d => d.direction).Distinct().OrderBy(dir => dir).ToArray();

            var directionIndex = 0;
            (int, int) result = (int.MinValue, int.MinValue);

            for (int i = 0; i < 200; i++)
            {
                var firingDirection = sortedDirections[directionIndex];
                var asteroidsTargeted = distancesAndDirections.Where(d => d.direction == firingDirection).OrderBy(d => d.distanceSquared).ToList();

                if (asteroidsTargeted.Any())
                {
                    var destroyedAsteroid = asteroidsTargeted.First();
                    distancesAndDirections.Remove(destroyedAsteroid);
                    result = destroyedAsteroid.otherAsteroid;
                }
                else
                {
                    i--;
                }

                directionIndex++;

                if (directionIndex > sortedDirections.Length)
                    directionIndex = 0;
            }

            return result;
        }

        private static List<(int X, int Y)> ProcessMap(string map)
        {
            var result = new List<(int X, int Y)>();
            int mapWidth = map.IndexOf('\r');
            
            string strippedMap = map.Replace("\r\n", "");

            for (int i = 0; i < strippedMap.Length; i++)
            {
                if (strippedMap[i] == '#')
                    result.Add((i % mapWidth, i / mapWidth));
            }

            return result;
        }
    }
}
