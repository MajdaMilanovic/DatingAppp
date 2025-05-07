using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateMessageDto
{

    public  string RecipientUsername { get; set; } = string.Empty;

    public required string Content { get; set; } = string.Empty;
}
