using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application.DTOs.Book.Commands.AddBook
{
    public class CreateCommandValidator : AbstractValidator<CreateBookCommand>
    {
        public CreateCommandValidator() { 
        
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100).WithMessage("Author cannot exceed 100 characters.");
            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .MaximumLength(13).WithMessage("ISBN cannot exceed 13 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        }
      
    }
}
