using System;
using System.Reflection;

namespace customerportalapi.Entities
{
    public class ContractInvitation : Contract
    {

        public ContractInvitation() : base()
        {

        }

        public ContractInvitation(Contract parent) : base()
        {
            ClonarPropiedades(parent, this);
        }

        public SMContract SmContract { get; set; }


        public static U ClonarPropiedades<T, U>(T src, U dest)
        {
            Type tipoOrigen = src.GetType();
            Type tipoDestino = dest.GetType();
            foreach (PropertyInfo property in tipoOrigen.GetProperties())
            {
                try
                {
                    PropertyInfo destProperty = tipoDestino.GetProperty(property.Name);
                    if (!destProperty.CanWrite) continue;
                    destProperty.SetValue(dest, property.GetValue(src, null), null);
                }
                catch (Exception e)
                {
                    //throw new Exception("Hubo un problema clonando propiedades: " + property.Name + " entre tipos: " + tipoOrigen.Name + " y " + tipoDestino.Name + ", e:" + e.Message);
                }
            }
            return dest;
        }
    }
}
