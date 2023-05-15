using NHibernate.Mapping.ByCode.Impl;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebCarteiraMvc.Models;

namespace WebCarteiraMvc.ObjetosDeValor
{
    public class ModelParaViewPessoa
    {

        [Required(ErrorMessage = "Este campo é de preenchimento obrigatório")]
        public virtual string Nome { get; set; }
        [DisplayName("E-mail")]
        [Required(ErrorMessage = "Este campo é de preenchimento obrigatório")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Digite um e-mail válido")]
        public virtual string Email { get; set; }
        [Required(ErrorMessage = "Este campo é de preenchimento obrigatório")]
        public virtual string Senha { get; set; }

        [Required(ErrorMessage = "Este campo é de preenchimento obrigatório")]
        public virtual double Salario { get; set; }

        [Required(ErrorMessage = "Este campo é de preenchimento obrigatório")]
        public virtual double Limite { get; set; }
        [Required(ErrorMessage = "Este campo é de preenchimento obrigatório")]

        public virtual double Minimo { get; set; }
        [Required(ErrorMessage = "Este campo é de preenchimento obrigatório")]
        public virtual double Saldo { get; set; }

        public List<Pessoa> Pessoa { get; set; }

        public ModelParaViewPessoa (List<Pessoa> pessoas)
        {
            Pessoa = pessoas;
        }
    }
}
