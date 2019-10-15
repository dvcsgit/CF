using System;
using Utility;

namespace Models.Shared
{
    public class MoveToTarget
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Define.EnumMoveDirection Direction { get; set; }
    }
}
