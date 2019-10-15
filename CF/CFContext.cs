using CF.Maps;
using CF.Maps.Maintenance;
using CF.Models;
using CF.Models.Maintenance;
using System.Data.Entity;

namespace CF
{
    public class CFContext:DbContext
    {
        public CFContext() : base("name=cf") { }

        public DbSet<Person> People { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<WebPermission> WebPermissions { get; set; }
        public DbSet<WebPermissionName> WebPermissionNames { get; set; }
        public DbSet<WebFunction> WebFunctions { get; set; }
        public DbSet<WebFunctionName> WebFunctionNames { get; set; }
        public DbSet<RolePermissionFunction> RolePermissionFunctions { get; set; }
        public DbSet<QueryableOrganization> QueryableOrganizations { get; set; }
        public DbSet<EditableOrganization> EditableOrganizations { get; set; }
        public DbSet<OrganizationManager> OrganizationManagers { get; set; }

        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EPart> EParts { get; set; }
        public DbSet<ESpecification> ESpecifications { get; set; }
        public DbSet<ESOption> ESOptions { get; set; }
        public DbSet<EquipmentSpecificationOption> EquipmentSpecificationOptions { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MSpecification> MSpecifications { get; set; }
        public DbSet<MSOption> MSOptions { get; set; }
        public DbSet<MaterialSpecificationOption> MaterialSpecificationOptions { get; set; }
        public DbSet<EquipmentMaterial> EquipmentMaterials { get; set; }
        public DbSet<EPartMaterial> EPartMaterials { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Checkpoint> Checkpoints { get; set; }
        public DbSet<CheckItem> CheckItems { get; set; }
        public DbSet<FeelOption> FeelOptions { get; set; }
        public DbSet<LineCheckpointEquipmentCheckItem> LineCheckpointEquipmentCheckItems { get; set; }
        public DbSet<LineCheckpointCheckItem> LineCheckpointCheckItems { get; set; }
        public DbSet<AbnormalReason> AbnormalReasons { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new PersonMap());
            modelBuilder.Configurations.Add(new RoleMap());
            modelBuilder.Configurations.Add(new OrganizationMap());
            modelBuilder.Configurations.Add(new WebPermissionMap());
            modelBuilder.Configurations.Add(new WebPermissionNameMap());
            modelBuilder.Configurations.Add(new WebFunctionNameMap());
            modelBuilder.Configurations.Add(new OrganizationManagerMap());
            modelBuilder.Configurations.Add(new RolePermissionFunctionMap());

            modelBuilder.Configurations.Add(new EquipmentMap());
            modelBuilder.Configurations.Add(new MaterialMap());
            modelBuilder.Configurations.Add(new EPartMap());
            modelBuilder.Configurations.Add(new ESpecificationMap());
            modelBuilder.Configurations.Add(new ESOptionMap());
            modelBuilder.Configurations.Add(new EquipmentSpecificationOptionMap());
            modelBuilder.Configurations.Add(new MSpecificationMap());
            modelBuilder.Configurations.Add(new MSOptionMap());
            modelBuilder.Configurations.Add(new MaterialSpecificationOptionMap());
            modelBuilder.Configurations.Add(new EquipmentMaterialMap());
            modelBuilder.Configurations.Add(new EPartMaterialMap());
            modelBuilder.Configurations.Add(new LineMap());
            modelBuilder.Configurations.Add(new CheckpointMap());
            modelBuilder.Configurations.Add(new LineCheckpointEquipmentCheckItemMap());
            modelBuilder.Configurations.Add(new LineCheckpointCheckItemMap());
            modelBuilder.Configurations.Add(new AbnormalReasonMap());
            modelBuilder.Configurations.Add(new CheckItemMap());
            modelBuilder.Configurations.Add(new JobMap());
        }
    }
}
