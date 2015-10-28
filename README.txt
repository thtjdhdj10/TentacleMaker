- Tentacle Maker 구조 -

1. 기본 구조

사용자가 준비한 GameObject 에 <TentacleRoot> 스크립트를 추가하여 생성할 수 있다.

이 스크립트를 통해 그 root 에 달린 TentacleBody 를 제어할 수 있다.

AddBodyObjects(List<GameObject> obj) 함수로 몸체를 연장시킬 수 있다.

인자로 들어간 오브젝트들에는 <TentacleBody> 스크립트가 추가되며, 이에 따라 transform 이 변한다.


TentacleRoot에는 촉수의 속성값인 TentacleProperties 을 public static Dictionary 로 가지고 있다.

public 이기 때문에 새로운 촉수의 속성을 정의하고 싶다면 직접 접근해서 수정하면 된다.

key 값은 string 이므로, 속성의 이름을 적당히 설정해서 쓰면 됨.


각 body 는 public properties 를 하나씩 가지고 있다. 이 값을 직접 접근하여 속성 값을 정하거나.

root 에 있는 SetTentacleProperties 함수로 설정하면 된다.



현재 구현된 촉수의 속성들은 아래와 같다.

	// 휜 정도의 회복 속도
        // recovery speed of bending
        public float bendingRecoverySpeed = 0.2f; // 0 ~ 1

        // 최대로 휘는 정도
        // max bending degrees of the two objects
        public float bendingDegrees = 7.5f;

        // 변형 회복 속도
        // form recovery speed by movement
        public float modifyRecoverySpeed = 0.2f; // 0 ~ 1

        // 변형에 의한 회전속도
        // rotation speed by modify
        public float rotationSpeedByModify = 0.5f; // 0 ~ 1

        // 최대 길이는 deltaTheta, deltaPhi 에 따라
        // baseLength + baseLength * lengthenDegrees 가 된다.

        // 길어지는 비율
        public float lengthenRatio = 2.0f;

        // object 사이의 최소 간격( lengthenRatio = 0 일때 길이는 이 값으로 고정됨 )
        public float minLength = 0.5f;

        public bool solid = false;

        // 일단 임시로 모든 실린더 on off 여기서 관리.
        // TODO body 에서 관리하게 수정
        public bool useCylinder = true;

        public string cylinder = "Cylinder";

        // TODO 얇아지는 비율

        // 굵기
        public float thickness = 1.0f;

        public float headSize = 1.0f;
        public float tailSize = 1.0f;

2. ReverseOperation() 에 대한 설명

ReverseOperation() 은 외부의 명령에 의해 body 의 형체가 바뀌었을 때,

그 형체에 대한 theta, phi 값을 다시 계산하여 입력하는 함수.


n번째와 n+1번째 마디의 위치관계를 가지고 구면좌표계 공식을 대입하여 계산한다.

하지만 이 결과값은 언제나 0 ~ 359 사이의 값을 가지는 문제가 있다.

이게 왜 문제냐면,


현재 구조 상에선 특정 마디의 theta, phi 값이 음수거나 360 을 넘는 경우가 생긴다.

때문에 예를들어, -30 이었던 theta 를 다시 계산했는데 330 으로 바꿔버리는 경우가 있다.

이를 맞춰주는 계산이 필요하다.


먼저 현재 theta, phi 값을 표준값(0~359)으로 뺀 값을 구한다. delta~

ex) -30 이었다면, 이 값의 표준값인 330 을 빼서 -360 이 구해진다.

그리고 n번째 마디의 계산에 의해 구해진 theta,phi 값을 각각 toTheta, toPhi 라고 하자.

이 toTheta, toPhi 에 앞의 deltaTheta, deltaPhi 를 더한 것이 알맞게 맞춰진 값이다.

현재의 theta, phi 값이 보정된 toTheta, toPhi 를 따라가게 하면 됨.


하지만 문제는 여기서 끝나지 않는다..

( 이렇게까지해야하나 싶을 수 있겠지만,

"각 마디의 값 범위가 0~359 를 벗어나는 것" 을

허용하는 이유에 대한 설명을 좀 더 나중에 설명함. )



위와 같이 구현했을 경우 다음과 같은 특수한 상황에서 문제가 발생한다.

> 말로 형용하기 어려우니 그림 참조.


아래와 같이 현재 방향에 영향을 주는 요소는 두가지가 있다.

이들의 수치는 속성값과 방향 차이 정도에 따라 결정되는데,

두 수치가 반대 방향으로 일치하게 되는 경우가 있다.

예를들어 한 방향으로 계속해서 회전시키는 경우,

마디 중간에 꼬인 부분이 있다면 꼬인 상태가 풀어지지 않고 그 상태로 계속 돈다.


이를 해결하기 위해 앞의 toTheta, toPhi(방향)을 이전 마디의 방향과 일치시킬 필요가 있다.

예를 들어 계산된 toTheta 가 -30 이고 이전 마디의 theta 가 -360 이라면,

toTheta 는 360 을 뺀 -390 이 되게 한다.


