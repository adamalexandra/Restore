using Stripe;
using API.Entities;

namespace API.Services;

public class DiscountService
{
  public DiscountService(IConfiguration config)
  {
    StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
  }

  public async Task<AppCoupon?> GetCouponFromPromoCode(string code)
  {
    code = code.Trim();
    
    var promotionService = new PromotionCodeService();

    var options = new PromotionCodeListOptions
    {
      Code=code
    };

    var promotionCodes = await promotionService.ListAsync(options);

    var promotionCode = promotionCodes.FirstOrDefault();

    if (promotionCode?.Promotion?.CouponId != null)
    {
      // In Stripe.net 50.x the coupon is nested under `Promotion` rather than on
      // the PromotionCode itself.  Grab the ID and fetch the coupon separately.
      var couponService = new CouponService();
      var coupon = await couponService.GetAsync(promotionCode.Promotion.CouponId);

      return new AppCoupon
      {
        Name = coupon.Name,
        AmountOff = coupon.AmountOff,
        PercentOff = coupon.PercentOff,
        CouponId = coupon.Id,
        PromotionCode = promotionCode.Code
      };
    }
    return null;
  }

  public async Task<long>CalculateDiscountFromAmount(AppCoupon appCoupon, long amount,
    bool removeDiscount=false)
  {
    var couponService = new CouponService();

    var coupon = await couponService.GetAsync(appCoupon.CouponId);

    if (coupon.AmountOff.HasValue && !removeDiscount)
    {
      return (long)coupon.AmountOff;
    }
    else if (coupon.PercentOff.HasValue && !removeDiscount)
    {
      return (long)Math.Round(amount *(coupon.PercentOff.Value / 100),
        MidpointRounding.AwayFromZero);
    }
    return 0;
  }
}