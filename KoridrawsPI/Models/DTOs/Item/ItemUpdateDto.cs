using System.ComponentModel.DataAnnotations;

public class ItemUpdateDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public double Preco { get; set; }

    public List<int> ImagensParaRemover { get; set; } = new List<int>();

    public List<IFormFile> NovasImagens { get; set; } = new List<IFormFile>();
}