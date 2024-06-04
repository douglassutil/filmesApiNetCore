using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Data.Dtos;

public class UpdateEnderecoDto
{
    [Required(ErrorMessage = "O campo do Logradouro é obrigatório")]
    public string Logradouro { get; set; }

    public int Numero { get; set; }
}
