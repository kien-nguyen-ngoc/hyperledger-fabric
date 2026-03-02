import java.util.Scanner;

public class SimpleCalculator {

    static Scanner scanner = new Scanner(System.in);

    public static void main(String[] args) {

        boolean running = true;

        while (running) {
            showMenu();

            int choice = readInt();

            if (choice == 1) {
                add();
            } else if (choice == 2) {
                subtract();
            } else if (choice == 3) {
                multiply();
            } else if (choice == 4) {
                divide();
            } else if (choice == 5) {
                running = false;
            } else {
                System.out.println("Invalid choice");
            }

            System.out.println();
        }

        System.out.println("Program ended");
        scanner.close();
    }

    static void showMenu() {
        System.out.println("==== SIMPLE CALCULATOR ====");
        System.out.println("1. Add");
        System.out.println("2. Subtract");
        System.out.println("3. Multiply");
        System.out.println("4. Divide");
        System.out.println("5. Exit");
        System.out.print("Choose: ");
    }

    static void add() {
        int a = readInt("Enter first number: ");
        int b = readInt("Enter second number: ");
        int result = a + b;
        System.out.println("Result = " + result);
    }

    static void subtract() {
        int a = readInt("Enter first number: ");
        int b = readInt("Enter second number: ");
        int result = a - b;
        System.out.println("Result = " + result);
    }

    static void multiply() {
        int a = readInt("Enter first number: ");
        int b = readInt("Enter second number: ");
        int result = a * b;
        System.out.println("Result = " + result);
    }

    static void divide() {
        int a = readInt("Enter first number: ");
        int b = readInt("Enter second number: ");

        if (b == 0) {
            System.out.println("Cannot divide by zero");
            return;
        }

        int result = a / b;
        System.out.println("Result = " + result);
    }

    static int readInt() {
        while (!scanner.hasNextInt()) {
            scanner.next();
            System.out.print("Enter a number: ");
        }
        return scanner.nextInt();
    }

    static int readInt(String message) {
        System.out.print(message);
        return readInt();
    }
}
