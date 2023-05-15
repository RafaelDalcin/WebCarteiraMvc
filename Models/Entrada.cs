using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebCarteiraMvc.Models
{
    public class Entrada
    {
        public virtual int Id { get; set; }
        public virtual DateTime Data { get; set; }
        public virtual string Descricao { get; set; }
        public virtual double Valor { get; set; }
        public virtual Pessoa Pessoa { get; set; }

        public Entrada()
        {

        }
        public Entrada( DateTime data, string descricao, double valor, Pessoa pessoa)
        {
            Data = data;
            Descricao = descricao;
            Valor = valor;
            Pessoa = pessoa;

        }

    }
}
