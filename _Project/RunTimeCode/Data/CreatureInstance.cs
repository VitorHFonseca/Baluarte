using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameCore.Data
{
    [Serializable]
    public class CreatureInstance
    {
        // 1.1 Metadados da Instância
        public string Instancia_UID { get; set; }
        public string ID_Especie { get; set; }
        public string Nome_Criatura { get; set; }
        public RarityType Raridade { get; set; }
        public ElementType Elemento { get; set; }

        // 1.2 Status Modificados por Variação Genética (IVs)
        // Salva apenas o multiplicador genético bruto para re-cálculos estruturados
        public Dictionary<StatType, float> IVs { get; set; }
        
        public float IndicePotencia { get; set; }

        // 1.3 Dados de Habilidades Sorteadas
        public string ID_PoderAtivo { get; set; }
        public string ID_PoderPassivo { get; set; }

        // Estado da Sessão
        public bool SelecionadoParaDefesa { get; set; }

        public CreatureInstance()
        {
            Instancia_UID = Guid.NewGuid().ToString();
            IVs = new Dictionary<StatType, float>();
            SelecionadoParaDefesa = false;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static CreatureInstance Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<CreatureInstance>(json);
        }
    }
}