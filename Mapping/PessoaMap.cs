using NHibernate.Mapping.ByCode;
using NHibernate;
using FluentNHibernate.MappingModel.ClassBased;
using NHibernate.Mapping.ByCode.Conformist;
using WebCarteiraMvc.Models;

namespace WebCarteiraMvc.Mapping
{
    public class PessoaMap : ClassMapping<Pessoa>
    {

        public PessoaMap()
        {
            Id(x => x.Id, x =>
            {
                x.Generator(Generators.Identity);
                x.Type(NHibernateUtil.Int32);
                x.Column("PES_CODIGO");
                
            });

            Property(x => x.Nome, x =>
            {
                x.Length(50);
                x.Type(NHibernateUtil.String);
                x.NotNullable(true);
                x.Column("PES_NOME");
            });

            Property(x => x.Email, x =>
            {
                x.Length(30);
                x.Type(NHibernateUtil.String);
                x.NotNullable(true);
                x.Column("PES_EMAIL");
            });

            Property(x => x.Senha, x =>
            {
                x.Length(30);
                x.Type(NHibernateUtil.String);
                x.NotNullable(true);
                x.Column("PES_SENHA");
            });

            Property(x => x.Salario, x =>
            {
                x.Type(NHibernateUtil.Double);
                x.NotNullable(false);
                x.Column("PES_SALARIO");
            });

            Property(x => x.Limite, x =>
            {
                x.Type(NHibernateUtil.Double);
                x.NotNullable(false);
                x.Column("PES_LIMITE");
            });

            Property(x => x.Minimo, x =>
            {
                x.Type(NHibernateUtil.Double);
                x.NotNullable(true);
                x.Column("PES_MINIMO");
            });

            Property(x => x.Saldo, x =>
            {
                x.Type(NHibernateUtil.Double);
                x.NotNullable(true);
                x.Column("PES_SALDO");

            });

            Table("Pessoas");
        }

    }
}
