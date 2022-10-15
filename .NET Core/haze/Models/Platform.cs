﻿namespace haze.Models
{
    public class Platform
    {
        public PlatformId Id { get; set; }
        public string Name { get; set; }
    }

    public enum PlatformId
    {
        PC,
        XBox,
        Playstation,
        Nintendo,
        Mobile
    }
}