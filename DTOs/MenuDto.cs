﻿
namespace SalafAlmoustakbalAPI.DTOs
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool HasChild { get; set; }
        public int ParentId { get; set; }
        public int BarId { get; set; }
    }
}
