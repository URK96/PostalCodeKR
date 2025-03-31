# PostalCodeKR (한국 우편번호 / 주소 검색 라이브러리리)
Republic of Korea Postal code search library

대한민국의 우편번호 / 주소 검색 라이브러리

## About Library (라이브러리에 대해)

This library is C# implementation of **'Postal Number Information'** open API service

본 라이브러리는 대한민국 공공데이터포털에서 제공하고 있는 **'과학기술정보통신부 우정사업본부_우편번호 정보조회'** 서비스의 C# 구현본입니다.

## Usage (사용법)

> [!WARNING]
> Get API key before use this library
> 
> 라이브러리 사용 전 API Key를 가져와야 합니다. 

> * Use static method (정적 함수 사용시)
>
> Use **PostalCodeSerivce.GetDatas** async method
>
> **PostalCodeService.GetDatas** 비동기 함수를 사용하세요
>
> ```
> ServiceResponseData result = await PostalCodeService.GetDatas(ServiceKey, "관악구", 10, 1);
> ```



