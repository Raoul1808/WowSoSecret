using System;
using System.Collections.Generic;

namespace WowSoSecret
{
    public static class ListExtensions
    {
        private static Random _random = new Random();
        
        public static T GetRandomOrDefault<T>(this List<T> list, T defaultValue) => list.Count <= 0 ? defaultValue : list[_random.Next(list.Count)];
    }
}
