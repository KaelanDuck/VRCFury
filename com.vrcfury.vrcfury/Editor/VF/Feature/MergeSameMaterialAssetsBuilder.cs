using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VF.Feature.Base;

namespace VF.Feature {
    public class MergeSameMaterialAssetsBuilder : FeatureBuilder {

        public class MaterialAssetComparer : IEqualityComparer<Material> {
            public bool Equals(Material a, Material b) {
                // trivial cases
                if (a == b) return true;
                if (a == null || b == null) return false;

                // check the basics
                if (a.shader != b.shader) return false;
                if (a.renderQueue != b.renderQueue) return false;
                if (a.doubleSidedGI != b.doubleSidedGI) return false;
                if (a.enableInstancing != b.enableInstancing) return false;
                if (a.globalIlluminationFlags != b.globalIlluminationFlags) return false;
                if (!a.shaderKeywords.SequenceEqual(b.shaderKeywords)) return false;

                // probably check other materialpropertytypes too
                string[] afloats = a.GetPropertyNames(MaterialPropertyType.Float);
                string[] anums = a.GetPropertyNames(MaterialPropertyType.Int);
                string[] avector = a.GetPropertyNames(MaterialPropertyType.Vector);
                string[] atextures = a.GetPropertyNames(MaterialPropertyType.Texture);

                string[] bfloats = b.GetPropertyNames(MaterialPropertyType.Float);
                string[] bnums = b.GetPropertyNames(MaterialPropertyType.Int);
                string[] bvector = b.GetPropertyNames(MaterialPropertyType.Vector);
                string[] btextures = b.GetPropertyNames(MaterialPropertyType.Texture);

                // using the shorthand for "b integers" shuts down copilot autocomplete due to offensive term (???)

                if (!afloats.SequenceEqual(bfloats)) return false;
                if (!anums.SequenceEqual(bnums)) return false;
                if (!avector.SequenceEqual(bvector)) return false;
                if (!atextures.SequenceEqual(btextures)) return false;

                if (!afloats.Select(x => a.GetFloat(x)).SequenceEqual(bfloats.Select(x => b.GetFloat(x)))) return false;
                if (!anums.Select(x => a.GetInt(x)).SequenceEqual(bnums.Select(x => b.GetInt(x)))) return false;
                if (!avector.Select(x => a.GetVector(x)).SequenceEqual(bvector.Select(x => b.GetVector(x)))) return false;
                if (!atextures.Select(x => a.GetTexture(x)).SequenceEqual(btextures.Select(x => b.GetTexture(x)))) return false;

                return true;
            }

            public int GetHashCode(Material m) {
                // hacky hashcode
                return (
                    m.shader.GetHashCode() ^ 
                    ((m.mainTexture == null) ? 0 : m.mainTexture.GetHashCode()) ^
                    m.renderQueue ^
                    m.color.GetHashCode()
                );
            }
        }
        

        [FeatureBuilderAction(FeatureOrder.MergeSameMaterialAssets)]
        public void Apply() {
            // Basically, find and group any materials that are identical, but different assets
            // and replace all instances of their use with the first asset of the group
            // This allows darks to detect they are the same material and merge them
            
            // This is not applied to mat swap animations because it's pointless
            // (darks won't merge them anyway, and asset bundles are compressed)

            // get all renderers and their materials
            var allRenderers = avatarObject.gameObject.GetComponentsInChildren<Renderer>(true);
            var allMaterials = allRenderers.SelectMany(x => x.sharedMaterials).Distinct().ToArray();

            // group up the material assets that are equal
            var materialGroups = allMaterials.GroupBy(x => x, new MaterialAssetComparer())
                .ToList();
            
            foreach (IGrouping<Material,Material> group in materialGroups) {
                // this is the material that everything in the group will be set to
                Material finalMaterial = group.Key;

                // exclude the final material from the group
                var mats = group.Except(new Material[] {finalMaterial}).ToList();

                // count==0 implies that there is only one material in the group
                if (mats.Count == 0) continue;

                // replace all found materials with the final material
                // todo: optimise this better probably
                foreach (Renderer r in allRenderers) {
                    r.sharedMaterials = r.sharedMaterials.Select(x => mats.Contains(x) ? finalMaterial : x).ToArray();
                }
            }
        }
    }
}

