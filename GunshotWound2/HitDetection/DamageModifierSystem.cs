namespace GunshotWound2.HitDetection {
    using Configs;
    using GTA;
    using GTA.Native;
    using PedsFeature;
    using PlayerFeature;
    using Scellecs.Morpeh;
    using Utils;
    using EcsWorld = Scellecs.Morpeh.World;

    public sealed class DamageModifierSystem : ISystem {
        private readonly SharedData sharedData;
        public EcsWorld World { get; set; }

        public DamageModifierSystem(SharedData sharedData) {
            this.sharedData = sharedData;
        }

        public void OnAwake() {
            sharedData.pauseService.PauseStateChanged += OnPauseStateChanged;
        }

        public void OnUpdate(float deltaTime) {
            UpdateDamageModifiers(MainConfig.DAMAGE_MODIFIER);
        }

        public void Dispose() {
            sharedData.pauseService.PauseStateChanged -= OnPauseStateChanged;
            ResetModifiers();
        }

        private void UpdateDamageModifiers(float modifier) {
            bool pedsEnabled = !sharedData.mainConfig.pedsConfig.UseVanillaHealthSystem;
            if (pedsEnabled) {
                Player player = Game.Player;
                PlayerEffects.SetMeleeDamageModifier(player, modifier);
                PlayerEffects.SetWeaponDamageModifier(player, modifier);
            }

            bool playerEnabled = !sharedData.mainConfig.playerConfig.UseVanillaHealthSystem;
            if (playerEnabled) {
                PedEffects.SetAiMeleeDamageModifier(modifier);
                PedEffects.SetAiWeaponDamageModifier(modifier);
            }

            if (pedsEnabled || playerEnabled) {
                float backModifier = 1f / modifier;
                SetDamageModifierForIgnoreSet(backModifier);
            }
        }

        private void SetDamageModifierForIgnoreSet(float modifier) {
            foreach (uint hash in sharedData.mainConfig.weaponConfig.IgnoreSet) {
                if (GTAHelpers.IsHumanWeapon(hash)) {
                    Function.Call(Hash.SET_WEAPON_DAMAGE_MODIFIER, hash, modifier);
                }
            }
        }

        private void ResetModifiers() {
            UpdateDamageModifiers(modifier: 1f);
            PedEffects.ResetMeleeDamageModifier();
            PedEffects.ResetWeaponDamageModifier();
        }

        private void OnPauseStateChanged(bool isPaused) {
            if (isPaused) {
                ResetModifiers();
            } else {
                OnUpdate(deltaTime: 0f);
            }
        }
    }
}