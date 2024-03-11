using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG.Saving {
    [System.Serializable]
    public struct CharacterMovementSaveData {
        public Vector3 position;
        public Vector3 rotation;

        public static JToken ToJToken(CharacterMovementSaveData data) {
            JObject obj = new JObject();
            obj["position"] = data.position.ToToken();
            obj["rotation"] = data.rotation.ToToken();
            return obj;
        }

        public static CharacterMovementSaveData FromJToken(JToken token) {
            CharacterMovementSaveData data = new CharacterMovementSaveData();
            if (token is JObject jObject) {
                data.position = jObject["position"].ToVector3();
                data.rotation = jObject["rotation"].ToVector3();
            }
            return data;
        }
    }
}