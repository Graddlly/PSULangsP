#include <Wire.h>
#include <LiquidCrystal.h>
#include <Bonezegei_DS1307.h>

Bonezegei_DS1307 rtc(0x68);
LiquidCrystal lcd(7, 6, 5, 4, 3, 2); // (rs, e, d4, d5, d6, d7)

void setup()
{
  Serial.begin(9600);
  lcd.begin(16, 2);   // Инициализация LCD 16x2
  lcd.print("RTC Time Only"); // Начальное сообщение

  rtc.begin();
  delay(500);
  lcd.clear();
}

void loop()
{
  if (rtc.getTime())
  {
    lcd.setCursor(4, 0); // Выводим на первую строку, начиная с 4-й колонки

    // Форматируем вывод ЧАСОВ с ведущим нулем
    if (rtc.getHour() < 10) lcd.print('0');
    lcd.print(rtc.getHour());

    lcd.print(":");

    // Форматируем вывод МИНУТ с ведущим нулем
    if (rtc.getMinute() < 10) lcd.print('0');
    lcd.print(rtc.getMinute());

    lcd.print(":");

    // Форматируем вывод СЕКУНД с ведущим нулем
    if (rtc.getSeconds() < 10) lcd.print('0');
    lcd.print(rtc.getSeconds());

    lcd.setCursor(0, 1);
    lcd.print("                ");
  }
  else
  {
    lcd.setCursor(0, 0);
    lcd.print("RTC Read Error!");
    lcd.setCursor(0, 1);
    lcd.print("                ");
  }

  delay(1000);
}