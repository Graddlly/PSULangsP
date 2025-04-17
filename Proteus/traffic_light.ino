// Определяем пины для светодиодов
const int redLedPin = 7;
const int yellowLedPin = 6;
const int greenLedPin = 5;

// Определяем время для каждой фазы (в миллисекундах)
const unsigned long redTime = 5000;
const unsigned long yellowTime = 2000;
const unsigned long redYellowTime = 2000;
const unsigned long greenTime = 5000;
const unsigned long blinkTime = 500;
const int blinkCount = 3;                 // Сколько раз мигнуть зеленым (вкл/выкл = 1 раз)

void setup() {
  // Настраиваем пины светодиодов как выходы
  pinMode(redLedPin, OUTPUT);
  pinMode(yellowLedPin, OUTPUT);
  pinMode(greenLedPin, OUTPUT);

  // Убедимся, что все светодиоды выключены при старте
  digitalWrite(redLedPin, LOW);
  digitalWrite(yellowLedPin, LOW);
  digitalWrite(greenLedPin, LOW);
}

void loop() {
  // --- ФАЗА 1: КРАСНЫЙ ---
  digitalWrite(redLedPin, HIGH);
  digitalWrite(yellowLedPin, LOW);
  digitalWrite(greenLedPin, LOW);
  delay(redTime);

  // --- ФАЗА 2: КРАСНЫЙ + ЖЕЛТЫЙ ---
  digitalWrite(yellowLedPin, HIGH);
  delay(redYellowTime);

  // --- ФАЗА 3: ЗЕЛЕНЫЙ ---
  digitalWrite(redLedPin, LOW);
  digitalWrite(yellowLedPin, LOW);
  digitalWrite(greenLedPin, HIGH);
  delay(greenTime);

  // --- ФАЗА 4: ЗЕЛЕНЫЙ МИГАЕТ ---
  for (int i = 0; i < blinkCount; i++) {
    digitalWrite(greenLedPin, LOW);
    delay(blinkTime);
    digitalWrite(greenLedPin, HIGH);
    delay(blinkTime);
  }
  
  // --- ФАЗА 4: ЖЕЛТЫЙ ---
  digitalWrite(yellowLedPin, HIGH);
  digitalWrite(greenLedPin, LOW);
  delay(yellowTime);

  digitalWrite(yellowLedPin, LOW);
}