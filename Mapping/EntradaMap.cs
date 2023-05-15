using FluentNHibernate.MappingModel.ClassBased;
using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using WebCarteiraMvc.Models;

namespace WebCarteiraMvc.Mapping
{
    public class EntradaMap : ClassMapping<Entrada>
    {

        public EntradaMap()
        {
            Id(x => x.Id, x =>
            {
                x.Generator(Generators.Identity);
                x.Type(NHibernateUtil.Int32);
                x.Column("ENT_CODIGO");
            });

            ManyToOne(x => x.Pessoa, x =>
            {
                x.Column("PES_CODIGO");
                x.NotNullable(true);
            });

            Property(x => x.Data, x =>
            {
                x.Type(NHibernateUtil.DateTime);
                x.NotNullable(true);
                x.Column("ENT_DATA");
            });

            Property(x => x.Descricao, x =>
            {
                x.Length(50);
                x.Type(NHibernateUtil.String);
                x.Column("ENT_DESCRICAO");
                x.NotNullable(true);
            });

            Property(x => x.Valor, x =>
            {
                x.Type(NHibernateUtil.Double);
                x.NotNullable(true);
                x.Column("ENT_VALOR");
            });

            Table("Entradas");
        }
    }
}

