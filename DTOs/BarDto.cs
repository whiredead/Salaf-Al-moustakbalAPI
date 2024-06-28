using System.ComponentModel.DataAnnotations;

namespace SalafAlmoustakbalAPI.DTOs
{
    public class BarDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Boolean hasChild { get; set; }

        public ICollection<MenuDto> MenusDto { get; set; }

    }
}