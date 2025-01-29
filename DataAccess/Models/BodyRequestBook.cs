using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models;

public class BodyRequestBook
{   
    [Required, Length(minimumLength: 3 , maximumLength: 250)]
    public string Title { get; set; } = string.Empty;
    
    [Required, Length(minimumLength: 3 , maximumLength: 250)]
    public string Author { get; set; } = string.Empty;
    
    [Required, Length(minimumLength: 1 , maximumLength: int.MaxValue)]
    public string Description { get; set; } = string.Empty;
}