namespace IntelligentVacuum.Environments
{
    using System;
    using System.Collections.Generic;
    using Environments;
    public sealed class Sensor
    {
        private AreaMap map;
        internal Sensor(AreaMap map)
        {
            this.map = map;
        }
        public Room SenseRoom(int x, int y)
        {
            if (x < 0 || y < 0)
            {
                return null;
            }
            if (x >= this.map.Rooms.GetLength(0) || y >= this.map.Rooms.GetLength(1))
            {
                return null;
            }
            return this.map.Rooms[x, y];
        }
    }
}
