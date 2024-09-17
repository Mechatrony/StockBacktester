namespace Kis.Dto;

public class 국내주식기간별시세Query(
  string FID_COND_MRKT_DIV_CODE,
  string FID_INPUT_ISCD,
  string FID_INPUT_DATE_1,
  string FID_INPUT_DATE_2,
  string FID_PERIOD_DIV_CODE,
  string FID_ORG_ADJ_PRC) {

  public string FID_COND_MRKT_DIV_CODE { get; set; } = FID_COND_MRKT_DIV_CODE;
  public string FID_INPUT_ISCD { get; set; } = FID_INPUT_ISCD;
  public string FID_INPUT_DATE_1 { get; set; } = FID_INPUT_DATE_1;
  public string FID_INPUT_DATE_2 { get; set; } = FID_INPUT_DATE_2;
  public string FID_PERIOD_DIV_CODE { get; set; } = FID_PERIOD_DIV_CODE;
  public string FID_ORG_ADJ_PRC { get; set; } = FID_ORG_ADJ_PRC;
}

public class 국내주식기간별시세DTO {
  //public string rt_cd { get; set; } = "";
  //public string msg_cd { get; set; } = "";
  //public string msg1 { get; set; } = "";
  //public string output1 { get; set; } = "";
  //public string prdy_vrss { get; set; } = "";
  //public string prdy_vrss_sign { get; set; } = "";
  //public string prdy_ctrt { get; set; } = "";
  //public string stck_prdy_clpr { get; set; } = "";
  //public string acml_vol { get; set; } = "";
  //public string acml_tr_pbmn { get; set; } = "";
  //public string hts_kor_isnm { get; set; } = "";
  //public string stck_prpr { get; set; } = "";
  //public string stck_shrn_iscd { get; set; } = "";
  //public string prdy_vol { get; set; } = "";
  //public string stck_mxpr { get; set; } = "";
  //public string stck_llam { get; set; } = "";
  //public string stck_oprc { get; set; } = "";
  //public string stck_hgpr { get; set; } = "";
  //public string stck_lwpr { get; set; } = "";
  //public string stck_prdy_oprc { get; set; } = "";
  //public string stck_prdy_hgpr { get; set; } = "";
  //public string stck_prdy_lwpr { get; set; } = "";
  //public string askp { get; set; } = "";
  //public string bidp { get; set; } = "";
  //public string prdy_vrss_vol { get; set; } = "";
  //public string vol_tnrt { get; set; } = "";
  //public string stck_fcam { get; set; } = "";
  //public string lstn_stcn { get; set; } = "";
  //public string cpfn { get; set; } = "";
  //public string hts_avls { get; set; } = "";
  //public string per { get; set; } = "";
  //public string eps { get; set; } = "";
  //public string pbr { get; set; } = "";
  //public string itewhol_loan_rmnd_ratem_name { get; set; } = "";

  // output2
  public string stck_bsop_date { get; set; } = "";
  public string stck_clpr { get; set; } = "";
  public string stck_oprc { get; set; } = "";
  public string stck_hgpr { get; set; } = "";
  public string stck_lwpr { get; set; } = "";
  public string acml_vol { get; set; } = "";
  public string acml_tr_pbmn { get; set; } = "";
  public string flng_cls_code { get; set; } = "";
  public string prtt_rate { get; set; } = "";
  public string mod_yn { get; set; } = "";
  public string prdy_vrss_sign { get; set; } = "";
  public string prdy_vrss { get; set; } = "";
  public string revl_issu_reas { get; set; } = "";
}
