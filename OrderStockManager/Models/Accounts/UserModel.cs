﻿using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using OrderStockManager.Infrastructure;
using System.Security.Claims;
using System.Threading.Tasks;
using OrderStockManager.Services;

namespace OrderStockManager.Models
{
    [Table("users")]
    public class UserModel : BaseModel, IUser<int>
    {
        #region IUser<int>
        [Key, Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("ログインＩＤ"), Column("code")]
        [Required, MaxLength(256)]
        [Index("ui_code", IsUnique = true)]
        public string UserName { get; set; }
        #endregion

        #region カスタム項目
        [DisplayName("ハッシュパスワード"), Column("password")]
        [DataType(DataType.Password)]
        [Required, MaxLength(256)]
        public string Password { get; set; }

        [DisplayName("ユーザー名"), Column("name")]
        [MaxLength(256)]
        public string Name { get; set; }

        [DisplayName("有効期限"), Column("expiration", TypeName = "Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Expiration { get; set; }

        [DisplayName("パスワード変更スキップ回数"), Column("password_skip_count")]
        [DefaultValue(0)]
        public int PasswordSkipCnt { get; set; }

        [DisplayName("資格情報"), Column("security_timestamp")]
        [MaxLength(256)]
        public string SecurityTimestamp { get; set; }

        [DisplayName("メールアドレス"), Column("email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress, MaxLength(128)]
        public string Email { get; set; }

        [DisplayName("メールアドレス確認済"), Column("email_confirmed")]
        [DefaultValue(false)]
        public bool EmailConfirmed { get; set; }

        [DisplayName("ロックアウト終了日時"), Column("lockout_end_data")]
        public DateTime? LockoutEndData { get; set; }

        [DisplayName("ロックアウト許可"), Column("lockout_enabled")]
        [DefaultValue(true)]
        public bool LockoutEnabled { get; set; }

        [DisplayName("アクセス失敗回数"), Column("access_failed_count")]
        [DefaultValue(0)]
        public int AccessFailedCount { get; set; }

        [DisplayName("使用許可"), Column("enabled")]
        [DefaultValue(false)]
        public bool Enabled { get; set; }
        #endregion

        #region 定型管理項目
        [DisplayName("削除済"), Column("deleted")]
        [DefaultValue(false)]
        public bool Deleted { get; set; }

        [DisplayName("作成日時"), Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDateTime { get; set; }

        [DisplayName("更新日時"), Column("modified_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifiedDateTime { get; set; }
        #endregion

        #region サポート機能
        [NotMapped]
        public ClaimsIdentity claims { get; set; }

        [NotMapped]
        public DateTimeOffset LockoutEndDataUtc
        {
            get
            {
                if (this.LockoutEndData != null)
                {
                    return DateTime.SpecifyKind((DateTime)this.LockoutEndData, DateTimeKind.Utc);
                }
                else
                {
                    return DateTimeOffset.MinValue;
                }
            }
            set
            {
                this.LockoutEndData = value.UtcDateTime;
            }
        }

        [NotMapped]
        public bool IsLockout
        {
            get
            {
                return LockoutEndDataUtc > DateTime.UtcNow;
            }
        }

        public ClaimsIdentity GenerateUserIdentity(UserManager<UserModel, int> manager, string authenticationType)
        {
            this.claims = manager.CreateIdentity(this, authenticationType);
            var stsId = ShortGuid.NewGuid();
            this.claims.AddClaim(new Claim("sts:id", stsId, ClaimValueTypes.String));
            var stsData = manager.GetSecurityStamp(Id);
            this.claims.AddClaim(new Claim("sts:ds", CryptographService.CreateMacCode(stsData, stsId), ClaimValueTypes.String));
            this.claims.AddClaim(new Claim(System.IdentityModel.Tokens.JwtRegisteredClaimNames.AuthTime, DateTime.UtcNow.Ticks.ToString(), ClaimValueTypes.Integer64));
            this.claims.AddClaim(new Claim("name", this.Name, ClaimValueTypes.String));

            // 不要なクレームを削除
            var delClaim = this.claims.Claims.Where(c => c.Type == "AspNet.Identity.SecurityStamp").SingleOrDefault();
            if (delClaim != null)
            {
                this.claims.TryRemoveClaim(delClaim);
            }
            delClaim = this.claims.Claims.Where(c => c.Type == "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider").SingleOrDefault();
            if (delClaim != null)
            {
                this.claims.TryRemoveClaim(delClaim);
            }
            return this.claims;
        }

        public Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<UserModel, int> manager, string authenticationType)
        {
            return Task.FromResult(GenerateUserIdentity(manager, authenticationType));
        }
        #endregion

        #region データ連携
        [JsonIgnore]
        public virtual ICollection<UserRoleModel> UserRoleModels { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserMakerModel> UserMakerModels { get; set; }

        [JsonIgnore]
        public virtual ICollection<SalesTrendModel> SalesTrendModels { get; set; }

        [JsonIgnore]
        public virtual ICollection<SignInLogModel> SignInLogModels { get; set; }
        #endregion
    }
}
