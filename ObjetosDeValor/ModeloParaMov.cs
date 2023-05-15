using com.sun.org.apache.xerces.@internal.impl.xpath.regex;
using System;
using System.ComponentModel.DataAnnotations;
using WebCarteiraMvc.Models;

namespace WebCarteiraMvc.ObjetosDeValor
{
    public class ModeloParaMov
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Valor { get; set; }
        public string Descricao { get; set; }
        public string TipoMovimentacao { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Data { get; set; }
        public Pessoa Pessoa { get; set; }
    }
}
