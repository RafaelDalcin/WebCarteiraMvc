using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebCarteiraMvc.Models
{
    public class Saida
    {
        public virtual int Id { get; set; }
        public virtual DateTime Data { get; set; }
        [Required]
        public virtual string Descricao { get; set; }
        [Required]
        public virtual double Valor { get ; set; }
        [Required]
        public virtual Pessoa Pessoa { get; set; }

        public Saida () 
        { 
        
        }
        public Saida( DateTime data, string desc, double valor, Pessoa pessoa)
        {

            Data = data;
            Descricao = desc;
            Valor = valor;
            Pessoa = pessoa;

        }

    }
}
