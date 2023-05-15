using NHibernate.Mapping;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebCarteiraMvc.Models
{
    public class Pessoa
    {
        public virtual string Senha { get; set; }
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual string Email { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public virtual double Salario { get; set; }
        public virtual double Limite { get; set; }
        public virtual double Minimo { get; set; }
        public virtual double Saldo { get;  set; }


        public Pessoa()
        {
    
        }
        

        public Pessoa(string nome, string email, double salario, double lim, double min, double saldo)
        {
            Nome = nome;
            Email = email;
            Salario = salario;
            Limite = lim;
            Minimo = min;
            Saldo = saldo;
        }

        public Pessoa (int id, string nome, string email, double salario, double lim, double min, double saldo) 
        {
            Id = id;
            Nome = nome;
            Email = email;
            Salario = salario;
            Limite = lim;
            Minimo = min;
            Saldo = saldo;
        }
    }
}
