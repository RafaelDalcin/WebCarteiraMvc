using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebCarteiraMvc.Models;

namespace WebCarteiraMvc.ObjetosDeValor
{
    public class ModeloParaViewMov
    {

        [Required(ErrorMessage = "Este campo é de preenchimento obrigatório")]
        public double Valor { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Limite máximo de 50 caractéres ultrapassado")]
        public string Descricao { get; set; }
        public DateTime From { get; set; } = DateTime.Now;
        public DateTime To { get; set; } = DateTime.Now;
        public bool CheckBoxEntrada { get;set; }
        public bool CheckBoxSaida { get; set; }

        public string Nome { get; set; }
        public List <ModeloParaMov> ModeloParaMov { get; set; }

        public List<Pessoa> Pessoa { get; set; }

        public ModeloParaViewMov(List<ModeloParaMov> movimentos, List<Pessoa> pessoa, string nome, bool checkentrada, bool checksaida, DateTime from, DateTime to)
        {
            From = from;
            To = to;
            CheckBoxEntrada = checkentrada;
            CheckBoxSaida = checksaida;
            ModeloParaMov = movimentos;
            Pessoa = pessoa;
            Nome = nome;
        }

        public ModeloParaViewMov(List<ModeloParaMov> movimentos, List<Pessoa> pessoa, string nome, bool checkentrada, bool checksaida)
        {
            CheckBoxEntrada = checkentrada;
            CheckBoxSaida = checksaida;
            ModeloParaMov = movimentos;
            Pessoa = pessoa;
            Nome = nome;
        }

        public ModeloParaViewMov(List<ModeloParaMov> movimentos, List<Pessoa> pessoa)
        {
            ModeloParaMov = movimentos;
            Pessoa = pessoa;
        }
        public ModeloParaViewMov(List<ModeloParaMov> movimentos)
        {
            ModeloParaMov = movimentos;
        }



    }
}
