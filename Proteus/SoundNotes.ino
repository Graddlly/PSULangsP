const int numKeys = 7; // Количество клавиш
const int buttonPins[numKeys] = {2, 3, 4, 5, 6, 7, 8}; // Пины для кнопок
const int speakerPin = 9; // Пин для пьезоизлучателя

const int noteFrequencies[numKeys] = {
  262, // До
  294, // Ре
  330, // Ми
  349, // Фа
  392, // Соль
  440, // Ля
  494  // Си
};

void setup() {
  for (int i = 0; i < numKeys; i++) {
    pinMode(buttonPins[i], INPUT_PULLUP);
  }

  pinMode(speakerPin, OUTPUT);
}

void loop() {
  bool notePlayed = false;

  for (int i = 0; i < numKeys; i++) {
    if (digitalRead(buttonPins[i]) == LOW) {
      tone(speakerPin, noteFrequencies[i]);
      notePlayed = true;
      break;
    }
  }

  if (!notePlayed) {
    noTone(speakerPin);
  }
}